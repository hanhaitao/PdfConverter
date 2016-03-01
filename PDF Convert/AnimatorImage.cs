using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace PDF_Convert
{
    ///        
    /// 表示一类带动画功能的图像。       
    ///        
    public class AnimateImage
    {
        Image image;
        FrameDimension frameDimension;
        ///        
        /// 动画当前帧发生改变时触发。       
        ///        
        public event EventHandler OnFrameChanged;

        ///        
        /// 实例化一个AnimateImage。       
        ///        
        /// 动画图片。       
        public AnimateImage(Image img)
        {
            image = img;
            lock (image)
            {
                mCanAnimate = ImageAnimator.CanAnimate(image);
                if (mCanAnimate)
                {
                    Guid[] guid = image.FrameDimensionsList;
                    //mFrameCount = image.GetFrameCount(frameDimension);
                }
            }
        }

        bool mCanAnimate;
        int mFrameCount = 1, mCurrentFrame = 0;

        ///        
        /// 图片。       
        ///        
        public Image Image
        {
            get { return image; }
        }

        ///        
        /// 是否动画。       
        ///        
        public bool CanAnimate
        {
            get { return mCanAnimate; }
        }

        ///        
        /// 总帧数。       
        ///        
        public int FrameCount
        {
            get { return mFrameCount; }
        }

        ///        
        /// 播放的当前帧。       
        ///        
        public int CurrentFrame
        {
            get { return mCurrentFrame; }
        }
        ///        
        /// 播放这个动画。       
        ///        
        public void Play()
        {
            if (mCanAnimate)
            {
                lock (image)
                {
                    ImageAnimator.Animate(image, new EventHandler(FrameChanged));
                }
            }
        }

        ///        
        /// 停止播放。       
        ///        
        public void Stop()
        {
            if (mCanAnimate)
            {
                lock (image)
                {
                    ImageAnimator.StopAnimate(image, new EventHandler(FrameChanged));
                }
            }
        }

        ///        
        /// 重置动画，使之停止在第0帧位置上。       
        ///        
        public void Reset()
        {
            if (mCanAnimate)
            {
                ImageAnimator.StopAnimate(image, new EventHandler(FrameChanged));
                lock (image)
                {
                    image.SelectActiveFrame(frameDimension, 0);
                    mCurrentFrame = 0;
                }
            }
        }

        private void FrameChanged(object sender, EventArgs e)
        {
            mCurrentFrame = mCurrentFrame + 1 >= mFrameCount ? 0 : mCurrentFrame + 1;
            lock (image)
            {
                image.SelectActiveFrame(frameDimension, mCurrentFrame);
            }
            if (OnFrameChanged != null)
            {
                OnFrameChanged(image, e);
            }
        }
    }
}
     

 

