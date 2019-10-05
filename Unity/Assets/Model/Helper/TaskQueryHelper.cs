using System;

namespace ETModel
{
    public static class TaskQueryHelper
    {
        public static async ETVoid OnTaskQueryAsync(Session session)
        {
            try
            {
                TaskQueryRsp rsp = (TaskQueryRsp) await session.Call(new TaskQueryReq() {});
                if (rsp.Error == (int)TaskQueryRsp.Types.ErrorCode.Succeed)
                {
                    Log.Info("task query succeed");
                } else 
                {
                    Log.Warning("unkown error");
                }
                // TODO: do something with GetTasks  DoneTasks, position_x、y
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 
    }
}