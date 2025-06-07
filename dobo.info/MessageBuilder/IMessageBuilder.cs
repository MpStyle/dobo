namespace dobo.info.MessageBuilder;

public interface IMessageBuilder
{
    Task<string?> Build(string? args);
}