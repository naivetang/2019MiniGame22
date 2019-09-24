﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    [Event(EventIdType.EnterGame)]
    public class EnterGame_CreateHall : AEvent
    {
        public override void Run()
        {
            //UIFactory.Create<UIHallComponent>(ViewLayer.UIMainLayer, UIType.UIHall).Coroutine();

            //UIFactory.Create<UIStartComponent>(ViewLayer.UIFullScreenLayer, UIType.UIStart).Coroutine();

            //UIFactory.Create<UIGuideSceneComponent>(ViewLayer.UIFullScreenLayer, UIType.UIGuideScene).Coroutine();

            UIFactory.Create<UICGComponent>(ViewLayer.UIFullScreenLayer, UIType.UICG).Coroutine();
        }
    }
}
