namespace FoodSnap.Application
{
    public class Error
    {
        public Error()
        {
        }

        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
