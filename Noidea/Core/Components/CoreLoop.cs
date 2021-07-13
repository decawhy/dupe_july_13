using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Noidea.Models;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000019 RID: 25
	public class CoreLoop : Component
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000092 RID: 146 RVA: 0x000024FD File Offset: 0x000006FD
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00002505 File Offset: 0x00000705
		public int OffsetTime { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00004C2C File Offset: 0x00002E2C
		public int Time
		{
			get
			{
				Stopwatch watch = this._watch;
				long? num = (watch != null) ? new long?(watch.ElapsedMilliseconds) : null;
				return ((num != null) ? new int?((int)num.GetValueOrDefault()) : null).GetValueOrDefault();
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004C84 File Offset: 0x00002E84
		public override void Initialize()
		{
			this._world = base.Client.GetComponent<World>();
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Ping>(new Action<Ping>(this.OnPing));
			base.Hook<Goto>(new Action<Goto>(this.OnGoto));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004CF0 File Offset: 0x00002EF0
		public void UsePortal(GameObject obj)
		{
			bool flag = base.Client.Position.DistanceTo(obj.Position) < 0.1f;
			if (flag)
			{
				Console.WriteLine("Using portal.2");
				base.Client.Send(new UsePortal
				{
					ObjectId = obj.ObjectId
				});
			}
			else
			{
				this.Target = new WorldPosData(obj.Position.X, obj.Position.Y);
				Action<Task> <>9__1;
				this.TargetCallback = delegate()
				{
					Console.WriteLine("Using portal.");
					Task task = Task.Delay(500);
					Action<Task> continuationAction;
					if ((continuationAction = <>9__1) == null)
					{
						continuationAction = (<>9__1 = delegate(Task t)
						{
							this.Client.Send(new UsePortal
							{
								ObjectId = obj.ObjectId
							});
						});
					}
					task.ContinueWith(continuationAction);
				};
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000250E File Offset: 0x0000070E
		private void OnGoto(Goto go)
		{
			base.Client.Send(new GotoAck
			{
				Time = this.FrameTime + this.OffsetTime
			});
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00002535 File Offset: 0x00000735
		private void OnPing(Ping ping)
		{
			base.Client.Send(new Pong
			{
				Time = this.Time + this.OffsetTime,
				Serial = ping.Serial
			});
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00002568 File Offset: 0x00000768
		private void OnCreateSuccess(CreateSuccess success)
		{
			this._watch = Stopwatch.StartNew();
			this.FrameTime = 0;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004DA8 File Offset: 0x00002FA8
		private void OnNewTick(NewTick tick)
		{
			foreach (ObjectStatusData objectStatusData in tick.Statuses)
			{
				bool flag = objectStatusData.ObjectId == base.Client.ObjectId;
				if (flag)
				{
					base.Client.ProcessData(objectStatusData.Stats);
				}
				else
				{
					Dictionary<int, GameObject> objects = this._world.Objects;
					lock (objects)
					{
						GameObject gameObject;
						bool flag3 = this._world.Objects.TryGetValue(objectStatusData.ObjectId, out gameObject);
						if (flag3)
						{
							gameObject.Position = objectStatusData.Position;
							gameObject.ProcessData(objectStatusData.Stats);
						}
					}
				}
			}
			int time = this.Time;
			this.GameTick(time - this.FrameTime);
			this.LastTickId = tick.TickId;
			this.FrameTime = time;
			base.Client.Send(new Move
			{
				NewPosition = base.Client.Position,
				RealTimeMS = tick.ServerRealTimeMS,
				Records = new MoveRecord[0],
				TickId = tick.TickId,
				Time = this.FrameTime + this.OffsetTime
			});
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004F08 File Offset: 0x00003108
		private void GameTick(int delta)
		{
			bool flag = this.Target != null;
			if (flag)
			{
				float num = this.GetSpeed() * (float)delta;
				float num2 = base.Client.Position.DistanceTo(this.Target);
				bool flag2 = num2 <= num;
				if (flag2)
				{
					base.Client.Position.X = this.Target.X;
					base.Client.Position.Y = this.Target.Y;
					this.Target = null;
					Action targetCallback = this.TargetCallback;
					if (targetCallback != null)
					{
						targetCallback();
					}
					this.TargetCallback = null;
				}
				else
				{
					float x = MathF.Atan2(this.Target.Y - base.Client.Position.Y, this.Target.X - base.Client.Position.X);
					base.Client.Position.X += MathF.Cos(x) * num;
					base.Client.Position.Y += MathF.Sin(x) * num;
				}
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000257D File Offset: 0x0000077D
		private float GetSpeed()
		{
			return 0.004f + (float)base.Client.Speed * 0.013333334f * 0.0056f;
		}

		// Token: 0x0400006A RID: 106
		public int FrameTime;

		// Token: 0x0400006B RID: 107
		public int LastTickId;

		// Token: 0x0400006C RID: 108
		public WorldPosData Target;

		// Token: 0x0400006D RID: 109
		public Action TargetCallback;

		// Token: 0x0400006E RID: 110
		private Stopwatch _watch;

		// Token: 0x0400006F RID: 111
		private World _world;
	}
}
