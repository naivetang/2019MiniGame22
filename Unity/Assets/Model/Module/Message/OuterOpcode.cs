using ETModel;
namespace ETModel
{
	[Message(OuterOpcode.LoginReq)]
	public partial class LoginReq : IRequest {}

	[Message(OuterOpcode.RegisterReq)]
	public partial class RegisterReq : IRequest {}

	[Message(OuterOpcode.RegisterRsp)]
	public partial class RegisterRsp : IResponse {}

	[Message(OuterOpcode.LoginRsp)]
	public partial class LoginRsp : IResponse {}

	[Message(OuterOpcode.TaskQueryReq)]
	public partial class TaskQueryReq : IRequest {}

//    int32 gid = 1;  // gid 没有必要，客户端没有要查询其他玩家进度信息的需求
	[Message(OuterOpcode.TaskQueryRsp)]
	public partial class TaskQueryRsp : IResponse {}

	[Message(OuterOpcode.TaskUpdateReq)]
	public partial class TaskUpdateReq : IRequest {}

	[Message(OuterOpcode.TaskUpdateRsp)]
	public partial class TaskUpdateRsp : IResponse {}

	[Message(OuterOpcode.C2M_TestRequest)]
	public partial class C2M_TestRequest : IActorLocationRequest {}

	[Message(OuterOpcode.M2C_TestResponse)]
	public partial class M2C_TestResponse : IActorLocationResponse {}

	[Message(OuterOpcode.Actor_TransferRequest)]
	public partial class Actor_TransferRequest : IActorLocationRequest {}

	[Message(OuterOpcode.Actor_TransferResponse)]
	public partial class Actor_TransferResponse : IActorLocationResponse {}

	[Message(OuterOpcode.C2G_EnterMap)]
	public partial class C2G_EnterMap : IRequest {}

	[Message(OuterOpcode.G2C_EnterMap)]
	public partial class G2C_EnterMap : IResponse {}

// 自己的unit id
// 所有的unit
	[Message(OuterOpcode.UnitInfo)]
	public partial class UnitInfo {}

	[Message(OuterOpcode.M2C_CreateUnits)]
	public partial class M2C_CreateUnits : IActorMessage {}

	[Message(OuterOpcode.Frame_ClickMap)]
	public partial class Frame_ClickMap : IActorLocationMessage {}

	[Message(OuterOpcode.M2C_PathfindingResult)]
	public partial class M2C_PathfindingResult : IActorMessage {}

	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.G2C_Test)]
	public partial class G2C_Test : IMessage {}

	[Message(OuterOpcode.C2M_Reload)]
	public partial class C2M_Reload : IRequest {}

	[Message(OuterOpcode.M2C_Reload)]
	public partial class M2C_Reload : IResponse {}

}
namespace ETModel
{
	public static partial class OuterOpcode
	{
		 public const ushort LoginReq = 101;
		 public const ushort LoginRsp = 102;
		 public const ushort RegisterReq = 103;
		 public const ushort RegisterRsp = 104;
		 public const ushort TaskQueryReq = 105;
		 public const ushort TaskQueryRsp = 106;
		 public const ushort TaskUpdateReq = 107;
		 public const ushort TaskUpdateRsp = 108;
		 public const ushort C2M_TestRequest = 109;
		 public const ushort M2C_TestResponse = 110;
		 public const ushort Actor_TransferRequest = 111;
		 public const ushort Actor_TransferResponse = 112;
		 public const ushort C2G_EnterMap = 113;
		 public const ushort G2C_EnterMap = 114;
		 public const ushort UnitInfo = 115;
		 public const ushort M2C_CreateUnits = 116;
		 public const ushort Frame_ClickMap = 117;
		 public const ushort M2C_PathfindingResult = 118;
		 public const ushort C2R_Ping = 119;
		 public const ushort R2C_Ping = 120;
		 public const ushort G2C_Test = 121;
		 public const ushort C2M_Reload = 122;
		 public const ushort M2C_Reload = 123;
	}
}
