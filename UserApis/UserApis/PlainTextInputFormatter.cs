using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace UserApis
{


        public class PlainTextInputFormatter : TextInputFormatter

        {

            public PlainTextInputFormatter()

            {

                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

                SupportedEncodings.Add(Encoding.UTF8);

                SupportedEncodings.Add(Encoding.Unicode);

            }

            protected override bool CanReadType(Type type)

            {

                return type == typeof(string);

            }

            public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)

            {

                using (var reader = new StreamReader(context.HttpContext.Request.Body, encoding))

                {

                    var text = await reader.ReadToEndAsync();

                    return await InputFormatterResult.SuccessAsync(text);

                }

            }

        }

        public class PlainTextOutputFormatter : TextOutputFormatter

        {

            public PlainTextOutputFormatter()

            {

                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

                SupportedEncodings.Add(Encoding.UTF8);

                SupportedEncodings.Add(Encoding.Unicode);

            }

            public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)

            {

                var response = context.HttpContext.Response;

                var responseText = context.Object.ToString(); // Assuming the object is a string

                return response.WriteAsync(responseText, selectedEncoding);

            }

        }

    }