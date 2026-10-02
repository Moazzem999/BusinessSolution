namespace BusinessSolution.Shared.Services
{
    public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor contextAccessor = contextAccessor;

        private int GetUserId()
        {
            var userId = CurrentContext?.User?.Claims?.FirstOrDefault(x => x.Type == "UserId")?.Value;

            if (userId is not null)
                return Convert.ToInt32(userId);

            return -1;
        }

        public HttpContext CurrentContext
        {
            get => GetCurrentContext();
        }

        private HttpContext GetCurrentContext()
        {
            return contextAccessor.HttpContext!;
        }

        public int UserId
        {
            get => GetUserId();
        }

    }

    public interface ICurrentUserService
    {
        public int UserId { get; }
    }
}
