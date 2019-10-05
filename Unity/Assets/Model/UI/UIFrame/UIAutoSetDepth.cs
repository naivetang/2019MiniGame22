﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETModel
{
    [RequireComponent(typeof(Image))]
    public class UIAutoSetDepth : UIBehaviour
    {
        private Image image;

        private Canvas canvas;

        private BoxCollider2D boxCollider2D;

        private int initDepth = 202;

        private int maxDepth = 240;
        
        void Start()
        {
            
            this.InitImage();
            
            this.InitCanvas();
           
            this.InitCollider();
            
        }

        void InitImage()
        {
            this.image = this.GetComponent<Image>();
        }

        void InitCanvas()
        {
            //this.initDepth = this.image.canvas.renderOrder;

            this.canvas = this.gameObject.AddComponent<Canvas>();

            this.canvas.overrideSorting = true;

            this.canvas.sortingOrder = this.initDepth;
        }

        void InitCollider()
        {
            this.boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();

            this.boxCollider2D.isTrigger = true;

            float offectY = (image.rectTransform.pivot.y-0.5f) * image.rectTransform.sizeDelta.y;

            float offectX = (image.rectTransform.pivot.x - 0.5f) * image.rectTransform.sizeDelta.x;

            this.boxCollider2D.offset = new Vector2(-offectX,-offectY);

            RectTransform rt = this.transform as RectTransform;

            this.boxCollider2D.size = rt.sizeDelta;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            //Log.Warning("进入 ： " + this.name );
            
            Transform colliderTransform = collision.transform;
            
            if (colliderTransform.tag.Equals("Player"))
            {
                if(colliderTransform.position.y > this.transform.position.y)
                    this.canvas.sortingOrder = this.maxDepth;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Log.Warning("离开 ： " + this.name);

            if (collision.gameObject.tag.Equals("Player"))
            {
                this.canvas.sortingOrder = this.initDepth;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            Transform colliderTransform = collision.transform;
            if (collision.gameObject.tag.Equals("Player"))
            {
                if (colliderTransform.position.y > this.transform.position.y)
                    this.canvas.sortingOrder = this.maxDepth;
                else
                    this.canvas.sortingOrder = this.initDepth;
            }
        }




    }
}
