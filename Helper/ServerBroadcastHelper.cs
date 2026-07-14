using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;

namespace EasyTools.Helper
{
    public static class ServerBroadcastHelper
    {
        public static IEnumerator<float> AutoServerBroadcast()
        {
            while (true)
            {
                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }

                //随机公告
                int tmp = UnityEngine.Random.Range(0, CustomEventHandler.Config.AutoServerMessageText.Count);

                Server.SendBroadcast(CustomEventHandler.Config.AutoServerMessageText[tmp], CustomEventHandler.Config.AutoServerMessageTimer, global::Broadcast.BroadcastFlags.Normal);
                yield return Timing.WaitForSeconds(CustomEventHandler.Config.AutoServerMessageTime * 60f);
            }
        }
    }
}
