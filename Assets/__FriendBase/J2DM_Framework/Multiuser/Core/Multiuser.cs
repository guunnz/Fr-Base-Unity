using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Injector.Core;
using System;
using UniRx;

namespace Multiuser
{
    public class Multiuser : IMultiuser
    {
        private Dictionary<string, ISubject<AbstractIncomingTrama>> tramaSubject;

        private Dictionary<string, List<Action<AbstractIncomingTrama>>> tramaDeliveries;
        private Dictionary<string, List<Action<AbstractTrama>>> tramaSendDeliveries;
        private IMultiuserInstance multiuserInstance;

        public Multiuser(IMultiuserInstance multiuserInstance)
        {
            tramaDeliveries = new Dictionary<string, List<Action<AbstractIncomingTrama>>>();
            tramaSendDeliveries = new Dictionary<string, List<Action<AbstractTrama>>>();

            this.multiuserInstance = multiuserInstance;
            //_multiuserInstance = Injection.Get<IMultiuserInstance>();
        }

        public void Send(AbstractTrama trama)
        {
            if (multiuserInstance != null)
            {
                DeliverSendTramaToSuscribers(trama);
                multiuserInstance.Send(trama);
            }
        }

        public void Connect(ConnectMultiuserParams parameters, UnityEngine.Object objectData)
        {
            if (multiuserInstance != null)
            {
                multiuserInstance.Connect(parameters, objectData);
            }
        }

        public bool ReConnect()
        {
            if (multiuserInstance != null)
            {
                return multiuserInstance.ReConnect();
            }
            return false;
        }

        public bool Disconnect()
        {
            if (multiuserInstance != null)
            {
                return multiuserInstance.Disconnect();
            }
            return false;
        }

        public ConnectionState GetConnectionState()
        {
            if (multiuserInstance != null)
            {
                return multiuserInstance.GetConnectionState();
            }
            return ConnectionState.NONE;
        }

        public void SetConnectionState(ConnectionState connectionState)
        {
            if (multiuserInstance != null)
            {
                multiuserInstance.SetConnectionState(connectionState);
            }
        }

        public IObservable<AbstractIncomingTrama> OnTrama(string tramaId)
        {
            if(tramaSubject.TryGetValue(tramaId, out var subject))
            {
                return subject;
            }

            subject = new Subject<AbstractIncomingTrama>();
            tramaSubject[tramaId] = subject;
            return subject;
        }

        public bool Suscribe(Action<AbstractIncomingTrama> suscriber, string[] tramaIds)
        {
            if (suscriber == null)
            {
                return false;
            }
            if (tramaIds == null || tramaIds.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < tramaIds.Length; i++)
            {
                string tramaId = tramaIds[i];
                if (!tramaDeliveries.ContainsKey(tramaId))
                {
                    tramaDeliveries[tramaId] = new List<Action<AbstractIncomingTrama>>();
                }
                tramaDeliveries[tramaId].Add(suscriber);
            }
            return true;
        }

        public bool Suscribe(Action<AbstractIncomingTrama> suscriber, string tramaId)
        {
            if (suscriber == null)
            {
                return false;
            }
            if (tramaId == null)
            {
                return false;
            }
            if (!tramaDeliveries.ContainsKey(tramaId))
            {
                tramaDeliveries[tramaId] = new List<Action<AbstractIncomingTrama>>();
            }
            tramaDeliveries[tramaId].Add(suscriber);

            return true;
        }

        public bool Unsuscribe(Action<AbstractIncomingTrama> suscriber, string[] tramaIds)
        {
            bool flag = false;
            if (suscriber == null)
            {
                return flag;
            }
            if (tramaIds == null || tramaIds.Length == 0)
            {
                return flag;
            }

            for (int i = 0; i < tramaIds.Length; i++)
            {
                string tramaId = tramaIds[i];
                if (tramaDeliveries.ContainsKey(tramaId))
                {
                    for (int j = tramaDeliveries[tramaId].Count - 1; j >= 0; j--)
                    {
                        if (tramaDeliveries[tramaId][j] == suscriber)
                        {
                            tramaDeliveries[tramaId].RemoveAt(j);
                            flag = true;
                        }
                    }
                }
            }
            return flag;
        }

        public bool Unsuscribe(Action<AbstractIncomingTrama> suscriber, string tramaId)
        {
            bool flag = false;
            if (suscriber == null)
            {
                return flag;
            }
            if (tramaId == null)
            {
                return flag;
            }
            if (tramaDeliveries.ContainsKey(tramaId))
            {
                for (int j = tramaDeliveries[tramaId].Count - 1; j >= 0; j--)
                {
                    if (tramaDeliveries[tramaId][j] == suscriber)
                    {
                        tramaDeliveries[tramaId].RemoveAt(j);
                        flag = true;
                    }
                }
            }

            return flag;
        }

        public void DeliverTramaToSuscribers(AbstractIncomingTrama trama)
        {
            if (trama == null)
            {
                return;
            }
            string tramaId = trama.TramaID;
            if (tramaDeliveries.ContainsKey(tramaId))
            {
                List<Action<AbstractIncomingTrama>> duplicateList = new List<Action<AbstractIncomingTrama>>(tramaDeliveries[tramaId]);
                int amount = duplicateList.Count;
                for (int i = 0; i < amount; i++)
                {
                    if (duplicateList[i] != null)
                    {
                        duplicateList[i](trama);
                    }
                }
            }
        }

        public void DeliverTriggerTramaToSuscribers(AbstractIncomingTrama trama)
        {
            if (trama == null)
            {
                return;
            }
            string tramaId = trama.TramaID;
            if(tramaSubject.TryGetValue(tramaId, out var subject))
            {
                subject.OnNext(trama);
            }
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //--------------  C A L L  B A C K  S E N D  T R A M A ----------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public bool SuscribeSendTrama(Action<AbstractTrama> suscriber, string tramaId)
        {
            if (suscriber == null)
            {
                return false;
            }
            if (tramaId == null)
            {
                return false;
            }
            if (!tramaSendDeliveries.ContainsKey(tramaId))
            {
                tramaSendDeliveries[tramaId] = new List<Action<AbstractTrama>>();
            }
            tramaSendDeliveries[tramaId].Add(suscriber);

            return true;
        }

        public bool UnsuscribeSendTrama(Action<AbstractTrama> suscriber, string tramaId)
        {
            bool flag = false;
            if (suscriber == null)
            {
                return flag;
            }
            if (tramaId == null)
            {
                return flag;
            }
            if (tramaSendDeliveries.ContainsKey(tramaId))
            {
                for (int j = tramaSendDeliveries[tramaId].Count - 1; j >= 0; j--)
                {
                    if (tramaSendDeliveries[tramaId][j] == suscriber)
                    {
                        tramaSendDeliveries[tramaId].RemoveAt(j);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public void DeliverSendTramaToSuscribers(AbstractTrama trama)
        {
            if (trama == null)
            {
                return;
            }
            string tramaId = trama.TramaID;
            if (tramaSendDeliveries.ContainsKey(tramaId))
            {
                List<Action<AbstractTrama>> duplicateList = new List<Action<AbstractTrama>>(tramaSendDeliveries[tramaId]);
                int amount = duplicateList.Count;

                for (int i = 0; i < amount; i++)
                {
                    duplicateList[i](trama);
                }
            }
        }
    }
}

