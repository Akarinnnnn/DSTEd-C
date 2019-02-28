﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DSTEd.Core.IO;

namespace DSTEd.Core.Klei {
    public class KleiGame {
        protected int id = -1;
        protected string name = null;
        protected string path = null;
        protected string executable = null;
        private Boolean is_main = false;
        private FileSystem files = null;

        public KleiGame() {}
        
        public string GetName() {
            return this.name;
        }

        public int GetID() {
            return this.id;
        }

        public Boolean IsMainGame() {
            return this.is_main;
        }

        public void SetMainGame() {
            this.is_main = true;
        }

        public void SetPath(string path) {
            this.path = path;
            this.Update();
        }

        private void Update() {
            this.files = new FileSystem(this.GetPath());
        }

        public FileSystem GetFiles() {
            return this.files;
        }

        public string GetPath() {
            return this.path;
        }

        public string GetExecutable() {
            return this.executable;
        }

        private MenuItem AddToolMenu(string name, string executable) {
            MenuItem item = new MenuItem();
            item.Name = name;
            item.Header = I18N.__(name);

            if (executable != null) {
                item.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                    try {
                        Process.Start(this.GetPath() + "/" + executable);
                    } catch {
                        Logger.Error("Can't open executable: " + this.GetPath() + "/" + executable);
                    }
                });
            }

            return item;
        }

        public void AddTool(string name, string executable) {
            MenuItem item = AddToolMenu(name, executable);
            MenuItem tools = Boot.Core().GetIDE().GetTools();
            tools.Items.Add(item);
        }

        public void AddSubTool(string node, string name, string executable) {
            MenuItem item = AddToolMenu(name, executable);
            MenuItem tools = Boot.Core().GetIDE().GetTools();
            MenuItem found = null;

            foreach (MenuItem entry in tools.Items) {
                if (entry.Name == node) {
                    found = entry;
                    break;
                }
            }

            if (found != null) {
                found.Items.Add(item);
            }
        }
    }
}
