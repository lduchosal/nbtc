use crate::encode::error::Error;
use crate::encode::encode::{Encodable, Decodable};
use crate::encode::varint::VarInt;


use std::io::{Read, Write, Cursor};
use byteorder::{LittleEndian, BigEndian, ReadBytesExt, WriteBytesExt};


#[derive(Debug, PartialEq)]
pub struct Inv {
    data: Vec<u8>,
}

impl Encodable for Inv {

    fn encode(&self, w: &mut Vec<u8>) -> Result<(), Error> {

        trace!("encode");
        let varint = VarInt::new(self.data.len() as u64);
        varint.encode(w)?;
        self.data.encode(w)?;
        Ok(())
    }
}

impl Decodable for Inv {

    fn decode(r: &mut Cursor<&Vec<u8>>) -> Result<Inv, Error> {

        trace!("decode");
        let varlen = VarInt::decode(r).map_err(|_| Error::InvLen)?;
        let mut data = vec![0u8; varlen.0 as usize];
        let mut data_ref = data.as_mut_slice();
        r.read_exact(&mut data_ref).map_err(|_| Error::InvMessage)?;

        let result = Inv {
            data: data
        };
        Ok(result)
    }
}

#[cfg(test)]
mod test {

    use crate::encode::encode::{Encodable, Decodable};
    use crate::network::inv::Inv;

    use std::io::Cursor;

    #[test]
    fn When_Encode_inv_Then_nothing_To_Encode() {

        let message = Inv {
            data: Vec::new()
        };
        let mut data : Vec<u8> = Vec::new();

        let result = message.encode(&mut data);
        assert!(result.is_ok());
        assert_eq!(1, data.len())
    }

    #[test]
    fn When_Decode_inv_Then_nothing_To_Encode() {

        let data : Vec<u8> = vec![ 0x0 ];
        let mut read = Cursor::new(&data);
        let result = Inv::decode(&mut read);

        let expected = Inv {
            data: Vec::new()
        };

        assert!(result.is_ok());
        assert_eq!(expected, result.unwrap());
    }

}