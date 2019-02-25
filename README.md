# MiniWebApi


MiniWebApi is a lightweight WebAPI framework for .NET which can help you build a system with WebAPI function quickly.


## Features
- [x] Support WebAPI protocol without anyother ASP.NET part.
- [x] Build-in http server.
- [x] Support [FromUrl] and [FromBody]
- [x] Use Emit to improve performance and reduce memory usage.
- [x] Signton pattern for request handler, each request will be handled by the same handler, and they are thread-safe.
- [x] Attributes support, almost the same as the MVC WebAPI。



## Code sample

Step 1. Define the handler for handling the WebAPI request.

```csharp
    public class TestObject
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
    
    [WebApiHandler("Test")]
    public class TestHandler:BaseHandler
    {

        [Get("GetMyObject")]
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
```
    
    
Step 2. Start the server which contains your handlers.
```csharp
        var server = new WebApiServer("api");
        server.Start(8090);
```

Contact mail: ithinkso117@163.com

[@Justin] 
