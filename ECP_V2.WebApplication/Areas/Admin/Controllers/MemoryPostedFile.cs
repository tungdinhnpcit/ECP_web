using System;
using System.IO;
using System.Web;

public class MemoryPostedFile : HttpPostedFileBase
{
    private readonly MemoryStream _stream;
    private readonly string _fileName;
    private readonly string _contentType;

    // Constructor nhận vào byte[] để tạo MemoryStream
    public MemoryPostedFile(byte[] fileBytes, string fileName, string contentType)
    {
        _stream = new MemoryStream(fileBytes); // Tạo MemoryStream từ byte[]
        _fileName = fileName;
        _contentType = contentType;
    }

    public override int ContentLength => (int)_stream.Length; // Lấy độ dài của stream
    public override string FileName => _fileName; // Tên tệp
    public override Stream InputStream => _stream; // Input stream (đọc tệp)
    public override string ContentType => _contentType; // Kiểu nội dung (Content-Type)

    // Đặt lại vị trí của stream về đầu
    public void ResetStream()
    {
        _stream.Seek(0, SeekOrigin.Begin);
    }
}
