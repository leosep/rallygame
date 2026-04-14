using System.IO;
using QRCoder;

namespace rallygame.Web.Services
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string data);
    }

    public class QRCodeService : IQRCodeService
    {
        public byte[] GenerateQRCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}