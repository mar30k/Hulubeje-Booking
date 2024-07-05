using QRCoder;

namespace HulubejeBooking.Controllers
{
    public interface IQRCodeGeneratorService
    {
        byte[] GenerateQRCode(string text);
    }

    public class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        public byte[] GenerateQRCode(string text)
        {
            byte[] qrCode = new byte[0];
            if (!string.IsNullOrEmpty(text))
            {
                QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
                qrCode = bitmap.GetGraphic(20);
            }
            return qrCode;
        }
    }
}