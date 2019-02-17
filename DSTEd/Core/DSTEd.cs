﻿using System;
using System.ComponentModel;
using DSTEd.Core.Klei.Games;
using DSTEd.UI;
using DSTEd.UI.Steam;

namespace DSTEd.Core {
    public class DSTEd : System.Windows.Application {
        private String version = "2.0.0";
        private String language = "en_US";
        private IDE ide = null;
        private Workspace workspace = null;
        private Loading loading = null;
        private Steam.Steam steam = null;
        private Configuration configuration = null;
        private Login login = null;

        public DSTEd() {
            Logger.Info("Start DSTEd v" + GetVersion());

            // Init classes
            this.login = new Login(this);
            this.configuration = new Configuration();
            this.ide = new IDE(this);
            this.workspace = new Workspace(this);
            this.loading = new Loading();
            this.steam = new Steam.Steam();

            // Set the steam path by configuration
            this.steam.SetPath(this.configuration.Get("STEAM_PATH", null));
            this.workspace.SetPath(this.steam.GetPath());

            this.workspace.OnSelect(delegate (string path, Boolean save) {
                if (!this.steam.ValidatePath(path)) {
                    Dialog.Open("Bad steam path! Please select the directory of Steam.", "Problem", Dialog.Buttons.OK, Dialog.Icon.Warning, delegate (Dialog.Result result) {
                        return true;
                    });
                    return;
                }

                this.steam.SetPath(path);
                this.workspace.SetPath(path);

                if (save) {
                    this.configuration.Set("STEAM_PATH", path);
                    this.configuration.Save();
                }

                this.workspace.Close(true);
                this.loading.Resume();
            });

            this.workspace.OnClose(delegate (CancelEventArgs e) {
                Dialog.Open("You must set the workspace path! If you cancel these, DSTEd will be closed.", "Problem", Dialog.Buttons.RetryCancel, Dialog.Icon.Warning, delegate (Dialog.Result result) {
                    if (result == Dialog.Result.Cancel) {
                        Environment.Exit(0);
                        return true;
                    }

                    e.Cancel = true;
                    return true;
                });
            });

            this.loading.OnSuccess(delegate () {
                Logger.Info("Steam path was set on", this.steam.GetPath());
                this.loading.Close();
                this.ide.Show();
            });

            // Adding workers to the loader...
            this.loading.Run("STEAM_PATH", delegate () {
                if (!this.steam.IsInstalled()) {
                    Logger.Info("Steam is not installed? Ask for Workspace...");

                    Dialog.Open("We can not find the path to STEAM. Please check the workspace settings.", "Problem: Steam", Dialog.Buttons.OK, Dialog.Icon.Warning, delegate (Dialog.Result result) {
                        this.workspace.Show();
                        return true;
                    });

                    return false;
                }

                this.workspace.SetPath(this.configuration.Get("STEAM_PATH", this.steam.GetPath()));
                Logger.Info("Steam-Path: " + this.workspace.GetPath());
                return true;
            });

            this.loading.Run("KLEI_GAMES", delegate () {
                this.steam.LoadGame(new DSTC());
                this.steam.LoadGame(new DSTS());
                this.steam.LoadGame(new DSTM());
                return true;
            });

            this.loading.Run("KLEI_MODS", delegate () {
                Logger.Info("Load mods...");
                return true;
            });

            this.loading.Start();
            this.Run();
        }

        public Login GetLogin() {
            return this.login;
        }

        public IDE GetIDE() {
            return this.ide;
        }

        public Steam.Steam GetSteam() {
            return this.steam;
        }

        public Workspace GetWorkspace() {
            return this.workspace;
        }

        public String GetVersion() {
            return this.version;
        }

        public String GetLanguage() {
            return this.configuration.Get("LANGUAGE", this.language);
        }
    }
}
