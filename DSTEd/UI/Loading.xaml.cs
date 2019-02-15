﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DSTEd.Core;

namespace DSTEd.UI {
    public partial class Loading : Window {
        private Action callback_success = null;
        private List<KeyValuePair<String, Func<Boolean>>> workers = new List<KeyValuePair<String, Func<Boolean>>>();
        private Boolean running = false;

        public Loading() {
            InitializeComponent();
        }

        public void SetProgress(int value) {
            Logger.Warn("Percent: ", value);
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate () {
                progress.Value = value;
            }));
        }

        public void Wait() {
            this.running = false;
        }

        public void Resume() {
            this.running = true;
        }

        public Boolean IsRunning() {
            return this.running;
        }

        public void Run(String name, Func<Boolean> callback) {
            this.workers.Add(new KeyValuePair<String, Func<Boolean>>(name, callback));
        }
        
        public void OnSuccess(Action callback) {
            this.callback_success = callback;
        }

        public void Start() {
            this.Resume();
            this.Show();

            int complete = this.workers.Count;
            int position = -1;

            Task.Run(() => {
                if (this.IsRunning()) {
                    ++position;
                    var entry               = this.workers[position];
                    String name             = entry.Key;
                    Func<Boolean> callback  = entry.Value;

                    Dispatcher.Invoke(delegate() {
                        if (!callback()) {
                            this.Wait();
                        }

                        this.SetProgress(position * 100 / complete);
                    });
                    
                    if (position >= complete) {
                        this.SetProgress(100);
                        this.callback_success();
                    }
                }

                Task.Delay(5000);
            });
        }
    }
}