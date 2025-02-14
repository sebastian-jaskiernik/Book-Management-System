namespace WebApplication3.Middleware
{
    public class LastVisitMiddleware
    {
        private readonly RequestDelegate _next;
        public readonly static string CookieName = "UNIVERSITY_LAST_VISIT";

        public LastVisitMiddleware(RequestDelegate @delegate)
        {
            _next = @delegate;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey(CookieName))
            {
                if (context.Request.Cookies.TryGetValue(CookieName, out string value))
                {
                    var visitDate = DateTime.Parse(value);
                    context.Items.Add(CookieName, visitDate);
                }
            }
            else
            {
                context.Items.Add(CookieName, "First visit.");
            }
            context.Response.Cookies.Append(CookieName, DateTime.Now.ToString());
            await _next(context);
        }
    }
}
