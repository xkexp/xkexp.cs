namespace TcpJson.Messages;

internal class Head
{
    public int body_length { get; set; }

    public int body_type_id { get; set; }

    public int message_id { get; set; }
}
