using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using ILRuntime.Runtime;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ETModel
{
    [ObjectSystem]
    public class UIShaddockAwakeSystem : AwakeSystem<UIShaddockSceneComponent>
    {
        public override void Awake(UIShaddockSceneComponent self)
        {
            self.Awake();
        }
    }
    
    [ObjectSystem]
    public class UIShaddockUpdateSystem : UpdateSystem<UIShaddockSceneComponent>
    {


        public override void Update(UIShaddockSceneComponent self)
        {
            self.Update();
        }
    }

    public class UIShaddockSceneComponent : Component
    {

        // ��ת����
        enum RockDir
        {
            Left,
            Right
        }
        
        private GameObject wall;

        private GameObject tree;

        private GameObject stick;

        private GameObject bevy;

        private GameObject whiteBG;

        private GameObject drawscene2;

        private GameObject context;
        
        private CancellationTokenSource cancellationTokenSource;

        private GameObject shootBtn;

        private GameObject tishiDialog;
        
        private Button cancel;

        private Vector3 stickInitPos;

        /// <summary>
        /// С�类�������ʾ����ͼ
        /// </summary>


        // ����Ķ�Ӧ�������¼�
        private int triggerId = 3002;



        private RockDir rockdir;
        
        // �����Ƿ�����ڶ�
        private bool bstickRock;

        private float minRockZ = -27f;
        private float maxRockZ = 16.8f;

        private float rotationZ;

        /// <summary>
        /// ��ת�ٶ�
        /// </summary>
        private float rotationSpeed;

        private UIShaddockSceneBind bind;
        
        
        private ShaddockChild leftChild;
        private ShaddockChild middleChild;
        private ShaddockChild rightChild;

        private ShaddockChild stickStayChild = null;

        private GameObject Shaddocks;

        private Vector3 stickInitScale;

        private GameObject middleDialog;
            
        private GameObject rightDialog;


        public void Awake()
        {
            
            if(Game.Scene.GetComponent<UnitComponent>().MyUnit != null)
                Game.Scene.GetComponent<UnitComponent>().MyUnit.RemoveComponent<UnitCameraFollowComponent>();
            
            ReferenceCollector rc = this.GetParent<UIBase>().GameObject.GetComponent<ReferenceCollector>();

            this.drawscene2 = rc.Get<GameObject>("drawscene2");

            this.wall = rc.Get<GameObject>("Wall");

            this.tree = rc.Get<GameObject>("Tree");

            this.stick = rc.Get<GameObject>("Stick");

            this.stickInitScale = this.stick.transform.localScale;

            this.bevy = rc.Get<GameObject>("Bevy");
            
            this.shootBtn = rc.Get<GameObject>("ShootBtn");
            
            this.shootBtn.SetActive(false);

            this.whiteBG = rc.Get<GameObject>("WhiteBG");
            
            this.Shaddocks = rc.Get<GameObject>("Shaddocks");

            this.drawscene2.SetActive(false);

            this.bind = this.GetParent<UIBase>().GameObject.GetComponent<UIShaddockSceneBind>();
            
            this.tishiDialog = rc.Get<GameObject>("tishiDialog");
            
            this.tishiDialog.SetActive(false);

            this.leftChild = new ShaddockChild(rc.Get<GameObject>("LeftChild"),this.tishiDialog, ChildType.Left);

            this.middleChild = new ShaddockChild(rc.Get<GameObject>("MiddleChild"),this.tishiDialog, ChildType.Middle);

            this.rightChild = new ShaddockChild(rc.Get<GameObject>("RightChild"),this.tishiDialog, ChildType.Right);

            this.stickInitPos = this.stick.transform.localPosition;
            
            this.cancel = rc.Get<GameObject>("Cancel").GetComponent<Button>();

            this.middleDialog = rc.Get<GameObject>("MiddleDialog");

            this.rightDialog = rc.Get<GameObject>("RightDialog");
            
            

            this.Init();
        }


        public void Update()
        {
            if (this.bstickRock)
            {
                this.UpdateStickRotation();
            }
        }

        void Init()
        {
            
            this.Addlistener();
            
            this.RegistStickDrag();

            //Game.EventSystem.Run(EventIdType.ShowJoystic);

            //Game.EventSystem.Run<int>(EventIdType.CompleteTask, this.triggerId);

            //this.CollectAndShow();
        }

        /// <summary>
        /// ע������϶��¼�
        /// </summary>
        void RegistStickDrag()
        {
            UIDragable drag = this.stick.GetComponent<UIDragable>();
            
            drag.RegistOnEndDrag(this.StickDragEnd);
        }

        void StickDragEnd(PointerEventData p)
        {
            if (this.stickStayChild != null && this.stickStayChild == this.leftChild)
            {
                Log.Info("���ܳɹ�");

                Game.EventSystem.UnRegisterEvent(EventIdType.ShaddockStickChild, this.stayChild);
                
                this.leftChild.UpdateState(ChildState.Ready);

                this.middleChild.UpdateState(ChildState.Ready);

                this.rightChild.UpdateState(ChildState.Ready);

                this.shootBtn.SetActive(true);

                middleDialog.GetComponent<DialogTextCtl>().SetText("  �����ӣ�  ", 2f);

                rightDialog.GetComponent<DialogTextCtl>().SetText("  �����ӣ�  ", 2f);

                // ��Ϳ�ʼ�ζ�
                this.cancellationTokenSource = new CancellationTokenSource();
            
                StartStickRotate(this.cancellationTokenSource.Token);
                
                // ���ø���tagֵ��ʹ���Ӵ������Ӻ���Ҷ���з���

                this.stick.transform.tag = "ShootStick";
            }
            else if (this.stickStayChild != null && this.stickStayChild == this.middleChild)
            {
                Log.Info("����ʧ��(�У�");


                Faild(this.middleChild).Coroutine();

                //this.reset(ChildType.Middle);
            }

            else if (this.stickStayChild != null && this.stickStayChild == this.rightChild)
            {
                Log.Info("����ʧ�ܣ��ң�");

                Faild(this.rightChild).Coroutine();

                //this.middleChild.UpdateState(ChildState.Fail);

                //this.reset(ChildType.Right);
            }
        }

        async ETVoid Faild(ShaddockChild child)
        {
            this.stick.SetActive(false);
                                                            
            child.UpdateState(ChildState.Fail);

            float leng = child.fail.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;

            TimerComponent time = Game.Scene.GetComponent<TimerComponent>();

            await time.WaitAsync((long) (leng * 1000));
            
            reset(child);
        }
        
        
        void reset(ShaddockChild child)
        {
            //this.stick.transform.localPosition = this.stickInitPos;

            // switch (type)
            // {
            //     case ChildType.Middle:
            //
            //         this.middleChild.UpdateState(ChildState.Jiemi);
            //                             
            //         break;
            //
            //     case ChildType.Right:
            //
            //         this.middleChild.UpdateState(ChildState.Jiemi);
            //
            //         break;
            // }
            
            child.UpdateState(ChildState.Jiemi);

            this.stick.transform.localPosition = this.stickInitPos;

            
            this.stick.SetActive(true);
        }


        private EventProxy stayChild;
        
        void Addlistener()
        {

            ExitScene();
                        
            this.shootBtn.GetComponent<Button>().onClick.AddListener(this.ShootButtonClick);
            
            this.stayChild = new EventProxy(this.StayChild);
            
            Game.EventSystem.RegisterEvent(EventIdType.ShaddockStickChild, this.stayChild);
            
        }

        void ExitScene()
        {

            this.bstickRock = false;

            this.cancel.onClick.AddListener(Close);
        }

        
        /// <summary>
        /// ������
        /// </summary>
        async void ShootButtonClick()
        {
            this.Shoot();

            this.ChildShootState();
            
            this.shootBtn.GetComponent<Button>().interactable = false;

            TimerComponent timer = Game.Scene.GetComponent<TimerComponent>();

            if (ShaddockTrigger.isComplete == true)
            {
                //����С���ӱ����е���Ч
                tree.GetComponent<ReferenceCollector>().Get<GameObject>("Shaddock").GetComponent<ReferenceCollector>().Get<GameObject>("AudioOuch").GetComponent<AudioSource>().Play();
            }      

            // һ��֮������³���
            await timer.WaitAsync(1 * 1000);

            //���ٴγ��͵�ʱ���ұ�����С����ΪReady״̬

            this.middleChild.UpdateState(ChildState.Ready);

            this.rightChild.UpdateState(ChildState.Ready);

            if (ShaddockTrigger.isComplete == true)
            {
                this.Complete();                              
            }
            else
            {
                this.shootBtn.GetComponent<Button>().interactable = true;

                this.leftChild.UpdateState(ChildState.Ready);

                this.cancellationTokenSource = new CancellationTokenSource();

                StartStickRotate(this.cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// ����ĸ����ӱ�����
        /// </summary>
        async void CheckShootShaddock()
        {
            int shaddockLayerMask = LayerMask.GetMask("Shaddock");//��ȡ��Ground���㼶
            
            var stickChild = this.stick.transform.Find("dir");

            GameObject middleDialog = this.GetParent<UIBase>().GameObject.GetComponent<ReferenceCollector>().Get<GameObject>("MiddleDialog");

            GameObject rightDialog = this.GetParent<UIBase>().GameObject.GetComponent<ReferenceCollector>().Get<GameObject>("RightDialog");

            RaycastHit hit;
            
            //�ж��Ƿ����
            bool isHit = false;

            int shaddockId = 0;

            if (Physics.Raycast(this.stick.transform.position, stickChild.position - this.stick.transform.position, out hit,
                50000,shaddockLayerMask))
            {
                Log.Info("�����ӣ�" + hit.collider.gameObject.name);
                
                shaddockId = hit.collider.gameObject.GetComponent<ShaddockTrigger>().GetShaddockId();

                isHit = true;


            }      
            
            if (isHit ==  true)
            {
                //���к󲥷����֣�ͬʱ����������С����ΪShoot״̬�������й���״̬��

                middleDialog.GetComponent<DialogTextCtl>().SetText("  ��������  ", 3f);

                rightDialog.GetComponent<DialogTextCtl>().SetText("  ������һ�Σ�  ", 3f);

                this.middleChild.UpdateState(ChildState.Shoot);

                this.rightChild.UpdateState(ChildState.Shoot);

                //��Shaddock����ȡ���ܻ���Ч������
                tree.GetComponent<ReferenceCollector>().Get<GameObject>("Shaddock").GetComponent<AudioSource>().Play();

            }
            else
            {
                middleDialog.GetComponent<DialogTextCtl>().SetText("  ��������  ", 3f);

                rightDialog.GetComponent<DialogTextCtl>().SetText("  ��׼�ˣ�  ", 3f);
            }
        
            Game.EventSystem.Run(EventIdType.ShaddockShootThing, shaddockId);
        }

        /// <summary>
        /// ����
        /// </summary>
        void Shoot()
        {

            this.bstickRock = false;

            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                
                this.cancellationTokenSource = null;
            }
            
            this.CheckShootShaddock();
            
            var stickChild = this.stick.transform.Find("dir");
            
            RaycastHit hit;
            
            if (Physics.Raycast(this.stick.transform.position, stickChild.position - this.stick.transform.position, out hit,1000))
            {
                //Log.Info("��ײ��" + hit.point);
                //Log.Info("��ײ��" + hit.collider.gameObject.name);

                 //this.GetParent<UIBase>().GameObject.transform.Find("Image11") .transform.position = hit.point;
                 
                 this.stick.transform.position = this.stickStayChild.stickShootPos.transform.position;

                 Vector2 endVec2 = hit.point;

                 Vector2 beginVec2 = this.stick.transform.position;
                 
                 float tan = (endVec2.y - beginVec2.y) / (endVec2.x - beginVec2.x);
                 
                 double angle=Mathf.Atan(tan) * 180 / 3.1415f;

                 if (angle < 0)
                 {
                     angle += 180;
                 }
                 
                 Log.Info("�Ƕȣ�" + angle);

                 Vector3 eulerAngles = this.stick.transform.eulerAngles;

                 eulerAngles.z = (float)angle;

                 this.stick.transform.eulerAngles = eulerAngles;

                 {

                     Vector3 scale = this.stick.transform.localScale;

                     float maxScale = 0.94f;

                     float minScale = 0.75f;
                     
                     if (angle > 90f)
                     {
                         scale.x = minScale;
                     }
                     else if (angle <= 39f)
                     {
                         scale.x = maxScale;
                     }
                     else
                     {
                         scale.x = (maxScale - minScale) / (90f - 39f) * (90f - (float) angle) + minScale;
                     }

                     Log.Info("scale X:" + scale.x);
                     
                     this.stick.transform.localScale = scale;
                 }
            }
        }

        void ChildShootState()
        {
            this.stickStayChild.UpdateState(ChildState.Shoot);
        }
        
        void StayChild(List<object> obj)
        {
            string action = obj[0] as string;
            ChildType type = (ChildType)obj[1];

            if (action.Equals("Enter"))
            {
                Log.Info("����С������С����" + type.ToString());

                if (type == ChildType.Left)
                    this.stickStayChild = this.leftChild;
                else if (type == ChildType.Middle)
                    this.stickStayChild = this.middleChild;
                else if (type == ChildType.Right)
                    this.stickStayChild = this.rightChild;
                
            }
            else if (action.Equals("Exit"))
            {
                Log.Info("�뿪С������С����" + type.ToString());

                this.stickStayChild = null;
            }
        }

        void RemoveListener()
        {
            Game.EventSystem.UnRegisterEvent(EventIdType.ShaddockStickChild, this.stayChild);
        }

        
        
        
        /// <summary>
        /// ����λ�ø���
        /// </summary>
         void UpdateStickRotation()
        {
            float speed = this.rotationSpeed * this.bind.addSpped[this.bind.speedIndex];

            float endZ = speed * Time.deltaTime;

            Vector3 angle = this.stick.transform.localEulerAngles;

            angle.z += this.rockdir == RockDir.Left? endZ : -endZ;
            
            this.stick.transform.localEulerAngles = angle;

            this.stick.transform.position = this.StickPos();


            //Log.Info("z:" + this.stick.transform.localEulerAngles);
            //Log.Info("��ǰλ�ã�" + angle.z);

            // TimerComponent timer = Game.Scene.GetComponent<TimerComponent>();
            //
            // while (true)
            // {
            //     await timer.WaitAsync((long)(0.))
            // }
        }

        Vector3 StickPos()
        {

            SkeletonAnimation graphic = this.leftChild.ready.GetComponent<SkeletonAnimation>();

            Bone bone = graphic.Skeleton.FindBone("right-hand2");
            
            //Log.Info($"({bone.WorldX},{bone.WorldY})" );
            
            
            //return new Vector3(bone.WorldX, bone.WorldY);

            //Vector3 vector3 = this.leftChild.self.transform.Find("Image").transform.position;
            
            //Log.Info("��ȷλ��Ӧ���� �� " + vector3.x + "," + vector3.y + "," + vector3.z);

            Vector3  vector3 = bone.GetWorldPosition(this.stickStayChild.ready.transform);

            return vector3;
            
            this.leftChild.self.transform.Find("Image").transform.position = vector3;
            
            return this.stickStayChild.stickIdlePos.transform.position;
        }

        /// <summary>
        /// ���ӿ�ʼ�ζ�
        /// </summary>
        async ETVoid StartStickRotate(CancellationToken cancellationToken)
        {
            this.bstickRock = true;
            
            TimerComponent timer = Game.Scene.GetComponent<TimerComponent>();

            this.stick.transform.localScale = this.stickInitScale;
            
            while (true)
            {
                if (this.IsDisposed)
                    return;
                
                Angle angle = GetNextAngle();

                rotationZ = angle.rotation;
            
                this.rotationSpeed = angle.speed;
                
                float waittime = this.GetWaitRotationTime();
                
                //Log.Info("��ʼ��ת��Ŀ��Ƕ�: " + this.rotationZ + ",  �ٶȣ�" + this.rotationSpeed + "��Ҫʱ����"  + waittime);

                //Log.Info("stick z��" + this.stick.gameObject.transform.localEulerAngles.z);
                
                

                if(this.rotationZ > this.stick.gameObject.transform.localEulerAngles.z)
                    rockdir = RockDir.Left;
                else
                    rockdir = RockDir.Right;
                
                //Log.Info("��ʼ��ת��Ŀ��Ƕ�: " + this.rotationZ + ",  �ٶȣ�" + this.rotationSpeed + "��Ҫʱ����"  + waittime + "  ����" + this.rockdir.ToString());
            
                await timer.WaitAsync((long)(waittime * 1000),cancellationToken);

                //this.bstickRock = false;

                //break;
            }
            
        }
        
        

        /// <summary>
        /// ��ת����һ������Ҫ��ʱ�䳤��
        /// </summary>
        /// <returns></returns>
        float GetWaitRotationTime()
        {
            float curr = this.stick.gameObject.transform.localEulerAngles.z;
            
            float target = rotationZ;

            float speed = this.rotationSpeed * this.bind.addSpped[this.bind.speedIndex];

            return  Mathf.Abs(target - curr) /speed;
        }
        
        Angle GetNextAngle()
        {
            this.bind.angleIndex += 1;

            return this.bind.Angles[this.bind.angleIndex % this.bind.Angles.Count];
        }

        void CloseOtherDrawScene()
        {
            this.wall.SetActive(false);

            this.tree.SetActive(false);

            this.stick.SetActive(false);

            this.bevy.SetActive(false);

            this.whiteBG.SetActive(false);

            this.rightDialog.SetActive(false);

            this.middleDialog.SetActive(false);
            //this.
        }

        void Complete()
        {
            Game.EventSystem.Run<int>(EventIdType.CompleteTask, this.triggerId);
            
            this.CollectAndShow();
        }

        async ETVoid CollectToBook()
        {
            UIBase com = UIFactory.Create<UIBookComponent>(ViewLayer.UIPopupLayer, UIType.UIBook).Result;

            com.GetComponent<UIBookComponent>().AddImageGo(this.drawscene2);


            this.Close();

        }

        void Close()
        {
            this.drawscene2 = null;

            Game.Scene.GetComponent<UIComponent>().RemoveUI(UIType.UIShaddockScene);
        }

        async ETVoid CollectAndShow()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            Log.Info(" ���ֽ���ͼ�� ");

            // ����
            this.drawscene2.SetActive(true);

            this.drawscene2.GetComponent<CanvasGroup>().alpha = 0;

            this.drawscene2.GetComponent<CanvasGroup>().DOFade(1, 1);

            await timerComponent.WaitAsync(1 * 1000);

            // ͼ����ȫ��ʾ����

            this.CloseOtherDrawScene();

            await timerComponent.WaitAsync(1 * 1000);

            // װ��������

            this.CollectToBook().Coroutine();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            Game.Scene.GetComponent<UnitComponent>().MyUnit.AddComponent<UnitCameraFollowComponent>();

            this.RemoveListener();
            
            this.leftChild?.Dispose();
            
            this.middleChild?.Dispose();
            
            this.rightChild?.Dispose();
            
            
        }

        #region ���С�к��߼�


        
        

        #endregion
    }
    
    public enum ChildState
    {
        None,
        Jiemi,//����
        JiemiPrompt,//������ʾ
        Ready,//׼��������
        Fail,//����ʧ��
        Shoot,//�����ӣ�����С���������ʱ�Ķ�����   
        Hit,//������
    }

    public enum ChildType
    {
        Left,
        Middle,
        Right,
    }

    public class ShaddockChild
    {
        public ChildType type; 
        
        public GameObject self;
        
        public GameObject shoot;
        public GameObject jiemi;
        public GameObject jiemiPrompt;
        public GameObject ready;
        public GameObject fail;
        public GameObject stickShootPos;

        public GameObject stickIdlePos;

        public ChildState state = ChildState.None;

        private GameObject tishiDialog;

        public ShaddockChild(GameObject go, GameObject tishi, ChildType type)
        {
            this.type = type;

            this.tishiDialog = tishi;
            Init(go);
        }

        public void Init(GameObject go)
        {
            this.self = go;
            
            ReferenceCollector rc = go.GetComponent<ReferenceCollector>();
            
            shoot = rc.Get<GameObject>("shoot");
            jiemi = rc.Get<GameObject>("jiemi");
            jiemiPrompt = rc.Get<GameObject>("jiemiPrompt");
            ready = rc.Get<GameObject>("ready");
            fail = rc.Get<GameObject>("fail");
            stickShootPos = rc.Get<GameObject>("stickShootPos");
            stickIdlePos = rc.Get<GameObject>("stickIdlePos");
            
            UpdateState(ChildState.Jiemi);
            
            Clicklistener();
            
            this.RegistColliderTrigger();
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        void Clicklistener()
        {
            this.self.GetComponent<UIPointHandler>().RegisterPointDown(this.SelfClick);
        }

        void SelfClick(PointerEventData p)
        {
            if (this.tishiDialog.activeSelf)
                return;

            this.tishiDialog.SetActive(true);
            
            this.Wait3sToHide().Coroutine();
        }

        async ETVoid Wait3sToHide()
        {
            TimerComponent timer = Game.Scene.GetComponent<TimerComponent>();

            await timer.WaitAsync(3 * 1000);

            if (this.tishiDialog != null)
            {
                this.tishiDialog.SetActive(false);
            }
        }
        
        

        /// <summary>
        /// ע�����򴥷��¼�
        /// </summary>
        void RegistColliderTrigger()
        {
            UIColliderTrigger trigger = this.self.GetComponent<UIColliderTrigger>();
            
            trigger.RegistOnTriggerEnter2D(this.TriggerEnter);
            
            trigger.RegistOnTriggerExit2D(this.TriggerExit);
        }

        void TriggerEnter(Collider2D c)
        {
            Game.EventSystem.Run(EventIdType.ShaddockStickChild, "Enter",this.type);
            
            UpdateState(ChildState.JiemiPrompt);
        }
        
        void TriggerExit(Collider2D c)
        {
            Game.EventSystem.Run(EventIdType.ShaddockStickChild, "Exit",this.type);
            
            UpdateState(ChildState.Jiemi);
        }
        
        public void UpdateState(ChildState state)
        {
            if (this.state == state)
                return;


            shoot.SetActive(false);
                   
            jiemi.SetActive(false);

            if(ready != null)
                this.ready.SetActive(false);

            if (fail != null)
                this.fail.SetActive(false);
            
            this.jiemiPrompt.SetActive(false);
            
            switch (state)
            {
                case ChildState.Jiemi:
                    
                    jiemi.SetActive(true);
                    
                    break;
                
                case ChildState.Ready:
                    
                    this.ready.SetActive(true);
                    
                    break;

                case ChildState.Fail:

                    this.fail.SetActive(true);

                    
                    break;
                
                case ChildState.Shoot:
                    
                    this.shoot.SetActive(true);
                    
                    break;
                
                case ChildState.JiemiPrompt:
                    
                    this.jiemiPrompt.SetActive(true);
                    
                    break;
            }
        }

        public void Dispose()
        {
            
        }
    } 
}

        


