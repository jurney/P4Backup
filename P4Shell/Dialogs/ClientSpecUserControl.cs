using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Perforce
{
	public partial class ClientSpecUserControl : UserControl
	{
		public ClientSpecUserControl()
		{			
			InitializeComponent();
			Clear();
		}
		

		public void Clear()
		{
			this.labelClient.Text	= "";
			this.labelUpdate.Text	= "";
			this.labelAccess.Text	= "";

			m_Client = null;
		}

		
		public string Client
		{
			get { return m_Client; }
			set
			{
				if( value == null )
				{
					Clear();					
				}
				else
				{
					ClientSpec clientSpec = new ClientSpec( value );

					if( P4Shell.GetClientSpec( value, ref clientSpec ) )
					{
						m_Client = value;

						this.labelClient.Text			= clientSpec.Client;
						this.labelUpdate.Text			= clientSpec.UpdateDate;
						this.labelAccess.Text			= clientSpec.AccessDate;
						this.textBoxOwner.Text			= clientSpec.Owner;
						this.textBoxHost.Text			= clientSpec.Host;
						this.textBoxDesc.Text			= clientSpec.Description;
						this.textBoxRoot.Text			= clientSpec.Root;
						this.checkBoxAllWrite.Checked	= clientSpec.AllWrite;
						this.checkBoxClobber.Checked	= clientSpec.Clobber;
						this.checkBoxCompress.Checked	= clientSpec.Compress;
						this.checkBoxLocked.Checked		= clientSpec.Locked;
						this.checkBoxModTime.Checked	= clientSpec.ModTime;
						this.checkBoxRmDir.Checked		= clientSpec.RmDir;

						// LineEnd ComboBox
						int lineEndIndex = this.comboBoxLineEnd.Items.IndexOf( clientSpec.LineEnd );
						if( lineEndIndex == -1 )
						{
							MessageBox.Show( "Invalid LineEnd encountered." );
							//this.comboBoxLineEnd.Enabled = false;
						}
						else
						{
							this.comboBoxLineEnd.SelectedIndex = lineEndIndex;
							//this.comboBoxLineEnd.Enabled = true;
						}

						this.textBoxView.Clear();
						foreach( string view in clientSpec.View )
						{
							this.textBoxView.Text += view + "\r\n";
						}
					}
					else
					{
						Clear();
					}
				}
			}
		}

		private string m_Client;
	}
}
