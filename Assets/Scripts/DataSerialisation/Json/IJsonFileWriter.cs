namespace DataSerialisation
{
    public interface IJsonFileWriter
    {
        void SerialiseData<T>(T data);
    }
}