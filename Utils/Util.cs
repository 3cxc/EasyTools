using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;

namespace EasyTools.Utils
{
    public class Util
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
                int tmp = new Random().Next(0, CustomEventHandler.Config.AutoServerMessageText.Count - 1);

                Server.SendBroadcast(CustomEventHandler.Config.AutoServerMessageText[tmp], CustomEventHandler.Config.AutoServerMessageTimer, global::Broadcast.BroadcastFlags.Normal);
                yield return Timing.WaitForSeconds(CustomEventHandler.Config.AutoServerMessageTime * 60f);
            }
        }
    }
}
