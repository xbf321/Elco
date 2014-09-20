using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class JobService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static JobInfo Create(JobInfo model) {
            if (model.Id == 0)
            {
                int id = JobManage.Insert(model);
                model.Id = id;
            }
            else {
                JobManage.Update(model);
            }
            return model;
        }
        /// <summary>
        /// 设置发布
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag">true:发布,false:不发布</param>
        /// <returns></returns>
        public static int SetPublish(int id, bool flag) {
            return JobManage.SetPublish(id,flag);
        }
         /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<JobInfo> List(JobSearchSetting setting)
        {
            return JobManage.List(setting);
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JobInfo Get(int id) {
            return JobManage.Get(id);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            return JobManage.Delete(id);
        }
    }
}
