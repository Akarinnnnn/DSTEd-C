using System;
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
		private void OnMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}
		public void Start(params Action[][] queue)
		{
			//Progress = 0;
			progress.Maximum = 0;
			foreach (var workers in queue)
			{
				progress.Maximum += workers.Length;
			}
			Show();
			foreach (Action[] workers in queue)
			{
				Parallel.ForEach(workers,
						(Action work) =>
						{
							work();
							progress.Dispatcher.Invoke(() => progress.Value++);
						}
					);
				#region DepercatedMT
				/*Thread[] threads = new Thread[workers.Length];
						for (int i = 0; i < workers.Length; i++) 
						{
							ThreadStart start = new ThreadStart(workers[i]);
							threads[i] = new Thread(start)
							{
		#pragma warning disable CS0618 //Set Apartmentsatate to STA forcely to operate UI.
								ApartmentState = ApartmentState.STA
							};
		#pragma warning restore CS0618
							threads[i].Start();
						}

						for (int i = 0; i < threads.Length; i++)
							if (threads[i].IsAlive) Thread.Yield();*/
				#endregion
			}

				while (progress.Value != progress.Maximum)
				Thread.Sleep(100);

			Close();
		}
	}
}
