using System;

namespace ClassLibrary1
{
    /// <summary>
    /// This is comment on base class
    /// </summary>
    public class Base
    {
        /// <summary>
        /// nice method
        /// </summary>
        public virtual void Hoge()
        {
        }
    }

    /// <inheritdoc/>
    public class Sub : Base
    {
        public override void Hoge()
        {
            base.Hoge();
        }
    }

    public interface IHoge
    {
        /// <summary>
        /// Do your work
        /// </summary>
        void Do();
    }

    public class Hoge : IHoge
    {
        // you dont need for interface, but you may not need add comment for warning.
        /// <inheritdoc/>
        public void Do()
        {
            throw new NotImplementedException();
        }
    }
}
