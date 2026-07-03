using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Net;
using System.Net.Mail;

namespace 发送邮件
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            SmtpClient smtp = new SmtpClient
            {
                //smtp服务器地址(我这里以126邮箱为例，可以依据具体你使用的邮箱设置)
                Host = "smtp.qq.com",
                Port = 587,
                EnableSsl = true,
               
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //这里输入你在发送smtp服务器的用户名和密码
                Credentials = new NetworkCredential("xxx@qq.com", "授权码")
            };
            //设置默认发送信息
            Email.DefaultSender = new SmtpSender(smtp);

            var email = Email
                
                //发送人
                .From("xxx@qq.com")
                //收件人
                .To("xxx@163.com")
                ////抄送人
                //.CC("admin@126.com")
                //邮件标题
                .Subject("自动占比数据")
            ///邮件内容
            .Body("2026-07-03", false);

            ////构建附件
            //// FileStream读取文件，复制到MemoryStream（用完可释放原文件流）
            //using var fs = new FileStream("C:\\Users\\arong\\Desktop\\报销单.xlsx", FileMode.Open, FileAccess.Read);
            //var ms = new MemoryStream();
            //fs.CopyTo(ms);
            //ms.Position = 0; // 重置流指针，否则读取为空

            //var attachment = new FluentEmail.Core.Models.Attachment
            //{
            //    Data = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "报销单.xlsx")),
            //    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //    //Filename = $"自动占比数据{DateTime.Now.ToString("yyyyMMdd")}.xlsx"
            //    Filename = "报销单.xlsx"
            //};
            ////添加附件
            //email.Attach(attachment);
            //var stream = new MemoryStream();
            //var sw = new StreamWriter(stream);
            //sw.WriteLine("您好,这是文本里的内容");
            //sw.Flush();
            //stream.Seek(0, SeekOrigin.Begin);
            //var attachment = new FluentEmail.Core.Models.Attachment
            //{
            //    Data = stream,
            //    ContentType = "text/plain",
            //    Filename = "Hello.txt"
            //};
            //email.AttachFromFilename(Path.Combine(AppContext.BaseDirectory, "报销单.xlsx"));

            var result =  email.Send();

            if (result.Successful)
            {
                //_logger.LogInformation("自动占比数据Email发送成功");
            }
            else
            {
                //_logger.LogError("自动占比数据Email发送失败: {Errors}", string.Join(", ", result.ErrorMessages));
            }
        }
    }
}
