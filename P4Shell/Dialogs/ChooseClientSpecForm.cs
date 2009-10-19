using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Perforce
{
	public partial class ChooseClientSpecForm : Form
	{
		public ChooseClientSpecForm( string currentClientSpec )
		{
			InitializeComponent();			

			// Show the info of the current client.
			this.clientSpecUserControl.Client = currentClientSpec;

			m_BackgroundWorker = new BackgroundWorker();

			m_BackgroundWorker.WorkerReportsProgress		= true;
			m_BackgroundWorker.WorkerSupportsCancellation	= true;
			m_BackgroundWorker.DoWork						+= new DoWorkEventHandler( this.EventDoWork );
			m_BackgroundWorker.ProgressChanged				+= new ProgressChangedEventHandler( this.EventProgressChanged );
			m_BackgroundWorker.RunWorkerCompleted			+= new RunWorkerCompletedEventHandler( this.EventRunWorkerCompleted );

			this.buttonCancel.Focus();
		}	


		private void ChooseClientSpecForm_Load( object sender, EventArgs e )
		{				
			this.listBoxClientSpecs.Enabled		= false;			
			this.progressBar.Value				= 0;
			this.buttonOK.Enabled				= false;

			m_BackgroundWorker.RunWorkerAsync();
		}


		private void EventDoWork( object sender, DoWorkEventArgs e )
        {
			List<string> clients = P4Shell.GetClients();

			if( this.listBoxClientSpecs.InvokeRequired )
			{
				this.listBoxClientSpecs.Invoke( new AddItemsDelegate( this.AddItems ), new object[] { clients } );
			}
			else
			{
				AddItems( clients );
			}
			
			for( int ii=0; ii<clients.Count; ++ii )
			{
				if( m_BackgroundWorker.CancellationPending )
				{
					e.Cancel = true;					
					break;
				}

				string		client		= clients[ii];
				ClientSpec	clientSpec	= new ClientSpec( client );

				if( P4Shell.GetClientSpec( client, ref clientSpec ) )
				{
					if( clientSpec.Owner != P4Shell.User )
					{
						if( this.listBoxClientSpecs.InvokeRequired )
						{
							this.listBoxClientSpecs.Invoke( new RemoveItemDelegate( this.RemoveItem ), new object[] { clientSpec.Client } );
						}
						else
						{
							RemoveItem( clientSpec.Client );
						}
					}
				}

				float progress = ii / (float)clients.Count;
				m_BackgroundWorker.ReportProgress((int)(progress * 100));
			}              
			
        }


		private void EventProgressChanged( object sender, ProgressChangedEventArgs e )
		{
			this.progressBar.Value = e.ProgressPercentage;
		}


		private void EventRunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
		{
			if( e.Cancelled == false )
			{
				int index = this.listBoxClientSpecs.Items.IndexOf( this.Client );

				if( index != -1 )
				{
					this.listBoxClientSpecs.SelectedIndex = index;
				}

				this.listBoxClientSpecs.Enabled		= true;				
				this.progressBar.Visible			= false;
				this.buttonOK.Enabled				= true;

				this.listBoxClientSpecs.Focus();
			}			
		}


		private void buttonCancel_Click( object sender, EventArgs e )
		{
			this.buttonCancel.Enabled	= false;
			this.buttonOK.Enabled		= false;

			m_BackgroundWorker.CancelAsync();

			this.DialogResult = DialogResult.Cancel;

			this.Close();
		}		


		private void buttonOK_Click( object sender, EventArgs e )
		{
			this.buttonCancel.Enabled	= false;
			this.buttonOK.Enabled		= false;

			m_BackgroundWorker.CancelAsync();			

			this.DialogResult = DialogResult.OK;

			this.Close();
		}


		private void ChooseClientSpecForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			m_BackgroundWorker.CancelAsync();
		}


		private void listBoxClientSpecs_SelectedIndexChanged( object sender, EventArgs e )
		{
			this.clientSpecUserControl.Client = (string)( this.listBoxClientSpecs.SelectedItem );
		}	


		private void AddItems( List<string> clients )
		{
			this.listBoxClientSpecs.Items.AddRange( clients.ToArray() );
		}


		private void RemoveItem( string client )
		{
			this.listBoxClientSpecs.Items.Remove( client );
		}


		public string Client
		{
			get { return this.clientSpecUserControl.Client; }
		}		

		
		private delegate void AddItemsDelegate( List<string> clients );
		private delegate void RemoveItemDelegate( string client );


		private BackgroundWorker m_BackgroundWorker;		
	}
}