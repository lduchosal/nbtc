using System;
using Nbtc.Network.Payload;
using Stateless;
using Version = Nbtc.Network.Payload.Version;

namespace NodeWalker.Business
{
    public class NodeWalkerStateMachine
    {
        enum Trigger
        {
            ConnectSocket,
            SendVersion,
            Retry,
            ReceiveVersion,
            ReceiveVerack,
            SendVerack,
            SetVersion,
            ReceiveOther,
            SendGetAddr,
            ReceiveAddr,
            Timeout
        };

        enum State
        {
            Init, 
            Connect, 
            VersionSent, 
            VersionReceived, 
            VerackReceived, 
            VerackSent, 
            Handshake, 
            GetAddr, 
            Addr
        };
        private readonly StateMachine<State, Trigger> _machine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<Addr> _addrTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<Version> _versionTrigger;

        private State _state;
        public event EventHandler OnInit = delegate { };
        public event EventHandler OnConnect = delegate { };
        public event EventHandler OnVersionSent = delegate { };
        public event EventHandler<Version> OnVersionReceived = delegate { };
        public event EventHandler OnVerackReceived = delegate { };
        public event EventHandler OnVerackSent = delegate { };
        public event EventHandler OnHandshake = delegate { };
        public event EventHandler OnGetAddr = delegate { };
        public event EventHandler<Addr> OnAddr = delegate { };
        public event EventHandler<string> OnUnhandledTrigger = delegate { };
        

        public NodeWalkerStateMachine()
        {
            _state = State.Init;
            
            var sm = new StateMachine<State, Trigger>(
                () => _state, 
                s => _state = s);
            
            var addrTrigger = sm.SetTriggerParameters<Addr>(Trigger.ReceiveAddr);
            var versionTrigger = sm.SetTriggerParameters<Version>(Trigger.ReceiveVersion);

            sm.Configure(State.Init)
                .OnEntry(() => OnInit(this, new EventArgs()))
                .Permit(Trigger.ConnectSocket, State.Connect);

            sm.Configure(State.Connect)
                .OnEntry(() => OnConnect(this, new EventArgs()))
                .Permit(Trigger.SendVersion, State.VersionSent);
            
            sm.Configure(State.VersionSent)
                .OnEntry(() => OnVersionSent(this, new EventArgs()))
                .Permit(Trigger.ReceiveVersion, State.VersionReceived)
                .PermitReentry(Trigger.Retry)
                ;
            
            sm.Configure(State.VersionReceived)
                .OnEntryFrom(versionTrigger, (v) => OnVersionReceived(this, v))
                .Permit(Trigger.ReceiveVerack, State.VerackReceived)
                .Permit(Trigger.SendVerack, State.VerackSent)
                ;
            
            sm.Configure(State.VerackReceived)
                .OnEntry(() => OnVerackReceived(this, new EventArgs()))
                .Permit(Trigger.SendVerack, State.VerackSent)
                ;

            sm.Configure(State.VerackSent)
                .OnEntry(() => OnVerackSent(this, new EventArgs()))
                .PermitReentry(Trigger.ReceiveVerack)
                .Permit(Trigger.SetVersion, State.Handshake)
                ;

            sm.Configure(State.Handshake)
                .OnEntry(() => OnHandshake(this, new EventArgs()))
                .Permit(Trigger.SendGetAddr, State.GetAddr)
                .Permit(Trigger.ReceiveAddr, State.Addr)
                .PermitReentry(Trigger.ReceiveOther)
                .PermitReentry(Trigger.Timeout)
                ;

            sm.Configure(State.GetAddr)
                .OnEntryFrom(Trigger.SendGetAddr, () => OnGetAddr(this, new EventArgs()))
                .PermitReentry(Trigger.ReceiveOther)
                .Permit(Trigger.ReceiveAddr, State.Addr)
                .Permit(Trigger.Timeout, State.Handshake)
                ;

            sm.Configure(State.Addr)
                .OnEntryFrom(addrTrigger, addr => OnAddr(this, addr) )
                ;


            sm.OnUnhandledTrigger((s,t) => OnUnhandledTrigger(this, $"{s} -> {t}"));
            _machine = sm;
            _addrTrigger = addrTrigger;
            _versionTrigger = versionTrigger;
        }

        public void ConnectSocket()
        {
            _machine.Fire(Trigger.ConnectSocket);   
        }
        
        public void SendVersion()
        {
            _machine.Fire(Trigger.SendVersion);   
        }
        
        public void ReceiveAddr(Addr a)
        {
            _machine.Fire<Addr>(_addrTrigger, a);   
        }
        
        public void ReceiveOther()
        {
            _machine.Fire(Trigger.ReceiveOther);   
        }
        public void ReceiveVerack()
        {
            _machine.Fire(Trigger.ReceiveVerack);   
        }
        
        public void ReceiveVersion(Version v)
        {
            _machine.Fire(_versionTrigger, v);   
        }
        public void SendVerack()
        {
            _machine.Fire(Trigger.SendVerack);   
        }
        public void SetVersion()
        {
            _machine.Fire(Trigger.SetVersion);   
        }
        public void SendGetAddr()
        {
            _machine.Fire(Trigger.SendGetAddr);   
        }
        public void Timeout()
        {
            _machine.Fire(Trigger.Timeout);   
        }
        
    }
}