using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ExemploMSMQ.Classes
{
    public static class QueueCore
    {
        private const string PATH_QUEUE_PROCESSO = @".\Private$\";

        private static MessageQueue LoadQueue(string pathQueue)
        {
            if (MessageQueue.Exists(pathQueue))
                return new MessageQueue(pathQueue);
            else
                return MessageQueue.Create(pathQueue);
        }

        public static MessageQueueTransaction CreateQueueTransaction()
        {
            return new MessageQueueTransaction();
        }

        public static System.Messaging.Message Send(string label, Object entity)
        {
            MessageQueue queueProcesso;
            MessageQueueTransaction queueTransaction = null;
            System.Messaging.Message msgEnvio = null;

            try
            {
                queueProcesso = LoadQueue(PATH_QUEUE_PROCESSO);

                msgEnvio = new Message();
                msgEnvio.Body = entity;
                msgEnvio.Label = label;

                queueTransaction = new MessageQueueTransaction();
                queueTransaction.Begin();
                queueProcesso.Send(msgEnvio, queueTransaction);
                queueTransaction.Commit();
            }
            catch (Exception)
            {
                if (queueTransaction.Status == MessageQueueTransactionStatus.Pending)
                    queueTransaction.Abort();

                msgEnvio = null;
            }

            return msgEnvio;
        }

        public static System.Messaging.Message Receive<T>(MessageQueueTransaction queueTransaction)
        {
            MessageQueue queueProcesso;
            System.Messaging.Message msgFila = null;

            try
            {
                queueProcesso = LoadQueue(PATH_QUEUE_PROCESSO);
                queueProcesso.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
                msgFila = queueProcesso.Receive(TimeSpan.Zero, queueTransaction);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    msgFila = null;
            }

            return msgFila;
        }
    }
}
