using Microsoft.Extensions.Logging;
using Nbtc.Network;
using System;

namespace Tests.Network
{
    public class VersionEncoder
    {
        private readonly ILogger _logger;

        public VersionEncoder(ILogger logger)
        {
            _logger = logger;
        }

        public Result<byte[]> Encode(Nbtc.Network.Version version)
        {

            return Result<byte[]>.Fail(ErrorEnum.Version);
            /*
            trace!("encode");
            self.version.encode(w).map_err(| _ | Error::VersionVersion) ?;
            self.services.encode(w).map_err(| _ | Error::VersionServices) ?;
            self.timestamp.encode(w).map_err(| _ | Error::VersionTimestamp) ?;
            self.receiver.encode(w).map_err(| _ | Error::VersionReceiver) ?;
            self.sender.encode(w).map_err(| _ | Error::VersionSender) ?;
            self.nonce.encode(w).map_err(| _ | Error::VersionNonce) ?;

            let user_agent_bytes = self.user_agent.as_bytes();
            let user_agent_len = user_agent_bytes.len() as u8;

            user_agent_len.encode(w).map_err(| _ | Error::VersionUserAgentLen) ?;

            w.write_all(user_agent_bytes).map_err(| _ | Error::VersionUserAgent) ?;

            self.start_height.encode(w).map_err(| _ | Error::VersionStartHeight) ?;
            self.relay.encode(w).map_err(| _ | Error::VersionRelay) ?;

            Ok(())
            return Result<byte[]>.Fail(ErrorEnum.Version);
            */
        }
    }
}