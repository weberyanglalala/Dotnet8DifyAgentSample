using Dotnet8DifyAgentSample.Services.LineMessage;
using Dotnet8DifyAgentSample.Settings;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi;

public class ChatController : LineWebHookControllerBase
{
    private readonly string _adminUserId;
    private readonly Bot _bot;
    private readonly LineMessageService _lineMessageService;

    public ChatController(LineMessagingApiSettings lineMessagingApiSettingsSettings, LineMessageService lineMessageService)
    {
        _lineMessageService = lineMessageService;
        _adminUserId = lineMessagingApiSettingsSettings.UserId;
        ChannelAccessToken = lineMessagingApiSettingsSettings.ChannelAccessToken;
        _bot = new Bot(ChannelAccessToken);
    }

    [Route("api/LineBotChatWebHook")]
    public async Task<IActionResult> GetChatResult()
    {
        try
        {
            if (IsLineVerify()) return Ok();
            foreach (var lineEvent in ReceivedMessage.events)
            {
                var lineUserId = lineEvent.source.userId;
                _bot.DisplayLoadingAnimation(lineEvent.source.userId, 20);
                var responseMessage = await _lineMessageService.ProcessMessageAsync(lineUserId, lineEvent.message.text);
                _bot.ReplyMessage(lineEvent.replyToken, responseMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _bot.PushMessage(_adminUserId, "系統忙碌中，請稍後再試。");
            return Ok();
        }

        return Ok();
    }

    private bool IsLineVerify()
    {
        return ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
               ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000";
    }
}