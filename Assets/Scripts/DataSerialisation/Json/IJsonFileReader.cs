namespace DataSerialisation 
{
    public interface IJsonFileReader
    {
        T ReadData<T>(string name = "");
    }
}