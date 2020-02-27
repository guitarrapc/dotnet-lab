using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    class SummaryComments
    {
        /// <summary>
        /// 1st
        /// 2nd line will not be in new line.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetHoge(int id)
        {
            return $"username for {id}";
        }

        /// <summary>
        /// <c>GetFuga</c> this is 1st line.
        /// <para>
        /// <see href="https://google.com">google.com</see>
        /// <list type="bullet">
        /// <item><description><para><em>bullet style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>bullet style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>bullet style, The unique user ID</em></para></description></item>
        /// </list>
        /// <list type="number">
        /// <item><description><para><em>number style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>number style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>number style, The unique user ID</em></para></description></item>
        /// </list>
        /// <list type="table">
        /// <item><description><para><em>table style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>table style, The unique user ID</em></para></description></item>
        /// <item><description><para><em>table style, The unique user ID</em></para></description></item>
        /// </list>
        /// </para>
        /// <returns><strong>return</strong> this will render</returns>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>this will not render</returns>
        public string GetFuga(int id)
        {
            return $"username for {id}";
        }
    }
}
