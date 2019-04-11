﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
namespace DSTEd.UI
{	
	public partial class Loadingv2 : Window
	{
		public Loadingv2()
		{
			InitializeComponent();
		}
		private Action[][] queueref;
		private int p = 0;
		public int Progress//0 to 1
		{
			get
			{
				return p;
			}
			set
			{
				p = value;
				Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
						new Action(() =>
						{
							progress.Value = value / queueref.Length * 100;
						})
				);
			}
		}
		private void OnMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}
		public void Start(params Action[][] queue)
		{
			queueref = queue;
			Progress = 0;
			Show();
			foreach (Action[] workers in queue)
			{
				Parallel.For(0, workers.Length, new Action<int>((int _p) => {
					workers[_p]();
					Progress++;
				}));
			}
			Hide();
		}
	}
}
