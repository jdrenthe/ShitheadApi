namespace ShitheadApi.Models
{
    /// <summary>
    /// This class carrys a T object
    /// carrys Content of a object if any
    /// carrys Sucess of the object
    /// carrys Error for the object if any
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataBundel<T>
    {
        public DataBundel(T SetContent, bool SetSuccess, string SetError)
        {
            if (SetContent != null)
            {
                Content = SetContent;
            }

            Success = SetSuccess;

            Error = SetError;
        }

        public T Content { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }
    }
}
