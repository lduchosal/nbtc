using System;
using Nbtc.Client;
using Nbtc.Network;
using Stateless;

namespace Nbtc.Serialization
{
    public class MessageStateMachine
    {
        private readonly StateMachine<StateEnum, Trigger>.TriggerWithParameters<long> _bytesTrigger;

        private readonly StateMachine<StateEnum, Trigger> _machine;
        private Message _message;
        private StateEnum _state;

        public StateEnum State
        {
            get { return _state; }
            private set
            {
                Console.WriteLine($"MessageStateMachine State [before : {_state}]");
                Console.WriteLine($"MessageStateMachine State [value : {value}]");
                _state = value;
            }
        }

        private Trigger? _error;


        public MessageStateMachine()
        {

            var sm = new StateMachine<StateEnum, Trigger>(
                () => State,
                s => State = s);

            var bytesTrigger = sm.SetTriggerParameters<long>(Trigger.Bytes);

            sm.Configure(StateEnum.None)
                .OnEntry(None)
                .PermitDynamic(bytesTrigger,
                    len => len >= 24
                        ? StateEnum.Message
                        : StateEnum.None
                )
                ;

            sm.Configure(StateEnum.Message)
                .OnEntry(Message)
                .Permit(Trigger.Bytes, StateEnum.Checksum)
                ;

            sm.Configure(StateEnum.Checksum)
                .OnEntry(Checksum)
                .PermitDynamic(bytesTrigger, (len) => StateEnum.Checksum)
                .Permit(Trigger.ChecksumSuceed, StateEnum.Payload)
                .Permit(Trigger.ChecksumFailed, StateEnum.Failed)
                ;

            sm.Configure(StateEnum.Payload)
                .OnEntry(Payload)
                .Permit(Trigger.PayloadSucceed, StateEnum.Succeed)
                .Permit(Trigger.PayloadFailed, StateEnum.Failed)
                ;

            sm.Configure(StateEnum.Succeed)
                .OnEntry(Succeed)
                .PermitDynamic(bytesTrigger,
                    len => len >= 24
                        ? StateEnum.Message
                        : StateEnum.None
                )
                ;

            sm.Configure(StateEnum.Failed)
                .OnEntryFrom(Trigger.PayloadFailed, () => _error = Trigger.PayloadFailed)
                .OnEntryFrom(Trigger.ChecksumFailed, () => _error = Trigger.ChecksumFailed)
                .OnEntryFrom(Trigger.FailedMessage, () => _error = Trigger.FailedMessage)
                ;

            sm.OnUnhandledTrigger(
                (s, t) =>
                    OnUnHandled(this, $"{s} - ({t}) -> undefined"));
            
            State = StateEnum.None;
            _message = new Message();
            _machine = sm;
            _bytesTrigger = bytesTrigger;
        }


        public event EventHandler<string> OnUnHandled = delegate { };
        public event EventHandler<MessageEventArgs> OnMessage = delegate { };
        public event EventHandler<MessageEventArgs> OnChecksum = delegate { };
        public event EventHandler<MessageEventArgs> OnPayload = delegate { };
        
        public MessageResult Bytes(long length)
        {
            if (length == 0)
            {
                return MessageResult.Missing();
            }

            
            var beforestate = _machine.State;
            _machine.Fire(_bytesTrigger, length);
            var afterstate = _machine.State;
            
            Console.WriteLine($"Bytes : [beforestate: {beforestate}]");
            Console.WriteLine($"Bytes : [afterstate: {afterstate}]");
            
            if (afterstate == StateEnum.Message
                || afterstate == StateEnum.Checksum)
            {
                return MessageResult.Missing();
            }
            else if ( afterstate == StateEnum.Succeed) 
            {
                return MessageResult.Succeed(_message);
            }
            
            return MessageResult.Failed(_error.ToString());

        }

        
        private void None()
        {
            _message = new Message();
            _error = null;
        }

        private void Message()
        {
            _message = new Message();
            _error = null;

            var mea = new MessageEventArgs {
                Message = _message,
            };
            OnMessage(this, mea);
            switch (mea.Result)
            {    
                case MessageStatut.Missing:
                    break;
                case MessageStatut.Succeed:
                    _machine.Fire(Trigger.Bytes);
                    break;
                default:
                    _machine.Fire(Trigger.FailedMessage);
                    break;
            }
        }

        private void Checksum()
        {
            var mea = new MessageEventArgs {
                Message = _message,
            };
            OnChecksum(this, mea);
            switch (mea.Result)
            {    
                case MessageStatut.Missing:
                    break;
                case MessageStatut.Succeed:
                    _machine.Fire(Trigger.ChecksumSuceed);
                    break;
                default:
                    _machine.Fire(Trigger.ChecksumFailed);
                    break;
            }
        }

        private void Payload()
        {
            
            var mea = new MessageEventArgs {
                Message = _message,
            };
            OnPayload(this, mea);
            switch (mea.Result)
            {    
                case MessageStatut.Missing:
                    break;
                case MessageStatut.Succeed:
                    _machine.Fire(Trigger.PayloadSucceed);
                    break;
                default:
                    _machine.Fire(Trigger.PayloadFailed);
                    break;
            }
        }

        private void Succeed()
        {

        }

        private enum Trigger
        {
            Bytes,
            ChecksumSuceed,
            PayloadSucceed,
            PayloadFailed,
            ChecksumFailed,
            FailedMessage,
        }

        public enum StateEnum
        {
            None,
            Message,
            Checksum,
            Payload,
            Succeed,
            Failed
        }

    }
}