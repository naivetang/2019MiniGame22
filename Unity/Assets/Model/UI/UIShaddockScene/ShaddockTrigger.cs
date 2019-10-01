using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ETModel
{
    
    /// <summary>
    /// 柚子被打
    /// </summary>
    public class ShaddockTrigger : UIBehaviour,IDisposable
    {
        //[SerializeField]
        private Animator animator;

        public static bool isComplete = false;

        public GameObject middleDialog;
        public GameObject rightDialog;

        public AudioClip dropAudio;

        [SerializeField]
        [CustomLabel("柚子ID")]
        private int Id;
        
        [SerializeField]
        [CustomLabel("掉落需要击中次数")]
        private int shootTimes;

        private string animationName;

        /// <summary>
        /// 当前被连续击中次数
        /// </summary>
        private int curShtTimes;

        private SkeletonGraphic animaton;

        private readonly string drop = "drop";
        private readonly string hitten = "hitten";
        private readonly string normal = "normal";
        private readonly string swing = "swing";
        
        void Start()
        {
            //this.animationName = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            animaton = this.gameObject.transform.Find("Image").GetComponent<SkeletonGraphic>();
            
            this.AddListener();
        }


        private EventProxy eventProxy;
        
        void AddListener()
        {
            eventProxy = new EventProxy(this.ShootThing);
            
            Game.EventSystem.RegisterEvent(EventIdType.ShaddockShootThing, this.eventProxy);
        }
        void RemoveListener()
        {
            if(this.eventProxy != null)
                Game.EventSystem.UnRegisterEvent(EventIdType.ShaddockShootThing, this.eventProxy);
        }
        
        void ShootThing(List<object> obj)
        {
            int shootId = (int) obj[0];

            if (shootId == this.Id)
                ++this.curShtTimes;
            else
                this.curShtTimes = 0;
            
            this.CheckState();
        }

        /// <summary>
        /// 每一次出杆之后，需要检查每个柚子状态，落下去？晃动？静止
        /// </summary>
        void CheckState()
        {
            float ID = GetShaddockId();                       

            // 静止
            if (this.curShtTimes == 0)
            {
                //Log.Info(Id + "静止");

                this.animaton.AnimationState.ClearTracks();
                
                this.animaton.AnimationState.SetAnimation(0, this.normal, false);
            }
            // 落下去
            else if (this.curShtTimes == this.shootTimes)
            {
                this.animaton.AnimationState.SetAnimation(0, this.drop, false);

                this.gameObject.transform.GetComponent<Collider>().enabled = false;

                if(ID > 4)
                {
                    Log.Info("Complete");

                    AudioSource.PlayClipAtPoint(dropAudio, this.gameObject.transform.localPosition);
                    //判定通关完成
                    isComplete = true;                    
                }
                else
                {
                    middleDialog.GetComponent<DialogTextCtl>().SetText("  掉进去了！  ", 2f);

                    rightDialog.GetComponent<DialogTextCtl>().SetText("  打这边的！  ", 2f);

                    AudioSource.PlayClipAtPoint(dropAudio, this.gameObject.transform.localPosition);
                                       
                }
            }
            // 晃动
            else 
            {
                this.animaton.AnimationState.SetAnimation(0, this.hitten, false);
                
                //animation.Skeleton.Data.Animations.Items[0].Duration;

                Spine.Animation a = this.animaton.Skeleton.Data.Animations.Find((t) => { return t.Name.Equals(this.normal); });

                this.animaton.AnimationState.AddAnimation(0, this.swing,true,0f);
            }
        }
        
        private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
        {
            //Log.Info("Enter");

            if (collision.gameObject.transform.tag.Equals("ShootStick"))
            {
                //Game.EventSystem.Run(EventIdType.ShaddockShootThing, Id);
            }
        }

        public int GetShaddockId()
        {
            return this.Id;
        }

        public void Dispose()
        {
            this.RemoveListener();
        }
    }
}