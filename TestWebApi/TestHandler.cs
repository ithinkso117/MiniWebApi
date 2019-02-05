using MiniWebApi.Handler;
using MiniWebApi.Network;

namespace TestWebApi
{
    public class TestObject
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    [WebApiHandler("Test")]
    public class TestHandler:BaseHandler
    {

        [Get]
        public void GetObject(WebApiHttpContext context)
        {
            context.Response.Write(new TestObject(){Name = "Justin", Age = 10});
        }

        [Get]
        public void Hello1(WebApiHttpContext context, string name, int age)
        {
            context.Response.Write($"Hello {name}, your age is {age}");
        }

        [Get]
        public void Hello2(WebApiHttpContext context, [FromUrl]TestObject obj)
        {
            context.Response.Write($"Hello {obj.Name}, your age is {obj.Age}");
        }

        [Post]
        public void Hello3(WebApiHttpContext context, string name, int age)
        {
            context.Response.Write($"Hello {name}, your age is {age}");
        }

        [Post]
        public void Hello4(WebApiHttpContext context, [FromBody]TestObject obj)
        {
            context.Response.Write($"Hello {obj.Name}, your age is {obj.Age}");
        }
    }
}


