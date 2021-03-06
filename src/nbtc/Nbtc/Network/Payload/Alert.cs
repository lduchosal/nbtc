﻿namespace Nbtc.Network.Payload
{
    /// <summary>
    /// 
    /// https://en.bitcoin.it/wiki/Protocol_documentation#alert
    /// 
    /// # alert
    /// 
    /// Note: Support for alert messages has been removed from bitcoin core in March 2016. Read more here
    /// https://bitcoin.org/en/alert/2016-11-01-alert-retirement
    /// The network wide Alert system is being retired. No Bitcoins are at risk and this warning
    /// may be safely ignored. Upgrade to the newest version of your walvar software to no longer 
    /// see the alert.
    /// 
    /// An alert is sent between nodes to send a general notification message throughout the network. 
    /// If the alert can be confirmed with the signature as having come from the core development group 
    /// of the Bitcoin software, the message is suggested to be displayed for end-users. 
    /// Attempts to perform transactions, particularly automated transactions through the client, 
    /// are suggested to be halted. The text in the Message string should be relayed to log files 
    /// and any user interfaces.
    /// 
    /// ## Alert format:
    /// 
    /// ```
    /// +------+-----------+---------+-----------------------------------+
    /// | Size | Descr     | Type    | Comments                          |
    /// +------+-----------+---------+-----------------------------------+
    /// |  ?   | payload   | uchar[] | Serialized alert payload          |
    /// +------+-----------+---------+-----------------------------------+
    /// |  ?   | signature | uchar[] | An ECDSA signature of the message |
    /// +------+-----------+---------+-----------------------------------+
    /// ```
    /// 
    /// The developers of Satoshi's client use this public key for signing alerts:
    /// 
    /// ```
    /// 04fc9702847840aaf195de8442ebecedf5b095cdbb9bc716bda9110971b28a49e
    /// 0ead8564ff0db22209e0374782c093bb899692d524e9d6a6956e7c5ecbcd68284
    /// (hash) 1AGRxqDa5WjUKBwHB9XYEjmkv1ucoUUy1s
    /// ```
    /// 
    /// The payload is serialized into a uchar[] to ensure that versions using incompatible 
    /// alert formats can still relay alerts among one another. The current alert payload 
    /// format is:
    /// 
    /// ```
    /// -----+-----------+------+----------------------------------------
    ///  Len | Descr     | Data | Comments                               
    /// -----+-----------+------+----------------------------------------
    ///  4   | Version   | i32  | Alert format version                           
    /// -----+-----------+------+----------------------------------------
    ///  8   | RelayUnti | i64  | The timestamp beyond which nodes      
    ///      |           |      | should stop relaying this alert       
    /// -----+-----------+------+----------------------------------------
    ///  8   | Expiratio | i64  | The timestamp beyond which this alert 
    ///      |           |      | is no longer in effect and should be  
    ///      |           |      | ignored                               
    /// -----+-----------+------+----------------------------------------
    ///  4   | ID        | i32  | A unique ID number for this alert     
    /// -----+-----------+------+----------------------------------------
    ///  4   | Cancel    | i32  | All alerts with an ID number less than
    ///      |           |      | or equal to this number should be     
    ///      |           |      | cancelled: deleted and not accepted   
    ///      |           |      | in the future                         
    /// -----+-----------+------+----------------------------------------
    ///  ?   | setCancel | si32 | All alert IDs contained in this set   
    ///      |           |      | should be cancelled as above          
    /// -----+-----------+------+----------------------------------------
    ///  4   | MinVer    | i32  | This alert only applies to versions   
    ///      |           |      | greater than or equal to this version.
    ///      |           |      | Other versions should still relay it. 
    /// -----+-----------+------+----------------------------------------
    ///  4   | MaxVer    | i32  | This alert only applies to versions   
    ///      |           |      | less than or equal to this version.   
    ///      |           |      | Other versions should still relay it. 
    /// -----+-----------+------+----------------------------------------
    ///  ?   | setSubVer | sstr | If this set contains any elements,    
    ///      |           |      | then only nodes that have their subVer
    ///      |           |      | contained in this set are affected by 
    ///      |           |      | the alert. Other versions should      
    ///      |           |      | still relay it.                       
    /// -----+-----------+------+----------------------------------------
    ///  4   | Priority  | i32  | Relative priority compared to other   
    ///      |           |      | alerts                                
    /// -----+-----------+------+----------------------------------------
    ///  ?   | Comment   | str  | A comment on the alert that is not    
    ///      |           |      | displayed                             
    /// -----+-----------+------+----------------------------------------
    ///  ?   | StatusBar | str  | The alert message that is displayed   
    ///      |           |      | to the user                           
    /// -----+-----------+------+----------------------------------------
    ///  ?   | Reserved  | str  | Reserved                              
    /// -----+-----------+------+----------------------------------------
    /// ```
    /// Note: set type in the table above is a variable length integer followed by the number of fields of the given type (either int32_t or variable length string)
    /// 
    /// ## Sample alert (no message header):
    /// 
    /// ```
    /// 73010000003766404f00000000b305434f00000000f2030000f10300000010270
    /// 00048ee00000064000000004653656520626974636f696e2e6f72672f66656232
    /// 3020696620796f7520686176652074726f75626c6520636f6e6e656374696e672
    /// 06166746572203230204665627275617279004730450221008389df45f0703f39
    /// ec8c1cc42c13810ffcae14995bb648340219e353b63b53eb022009ec65e1c1aae
    /// ec1fd334c6b684bde2b3f573060d5b70c3a46723326e4e8a4f1
    /// ```
    /// 
    /// 
    /// ```
    /// Version    : 1
    /// RelayUntil : 1329620535
    /// Expiration : 1329792435
    /// ID         : 1010
    /// Cancel     : 1009
    /// setCancel  : empty
    /// MinVer     : 10000
    /// MaxVer     : 61000
    /// setSubVer  : empty
    /// Priority   : 100
    /// Comment    : empty
    /// StatusBar  : "See bitcoin.org/feb20 if you have trouble connecting after 20 February"
    /// Reserved   : empty
    /// ```
    /// 
    /// </summary>
    public class Alert : IPayload
    {
        public byte[] Data { get; set; }
        public Command Command => Command.Alert;
    }
}