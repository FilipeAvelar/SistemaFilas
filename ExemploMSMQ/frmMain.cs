using ExemploMSMQ.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExemploMSMQ
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            NotaFiscal nf = new NotaFiscal();
            nf.numero = DateTime.Now.ToString("HHmmss");
            nf.valor = rand.Next(1, 5000);

            var retornoSend = QueueCore.Send(DateTime.Now.ToString("yyyyMMddHHmmss"), nf);

            //MessageBox.Show("Mensagem Enviada com Sucesso" + Environment.NewLine + String.Format("Id da Mensagem: {0}", retornoSend.Id));
        }

        private void btnReceber_Click(object sender, EventArgs e)
        {
            NotaFiscal nf = null;

            var queueTrans = QueueCore.CreateQueueTransaction();
            var msgFila = QueueCore.Receive<NotaFiscal>(queueTrans);

            if (msgFila != null)
            {
                nf = (NotaFiscal)msgFila.Body;
                MessageBox.Show(String.Format("Nota Fiscal: {0} - Valor: {1}", nf.numero, nf.valor));
            }
            else
            {
                MessageBox.Show("Não existem mensagens na fila");
            }

        }
    }
}
