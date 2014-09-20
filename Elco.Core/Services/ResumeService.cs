
using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class ResumeService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ResumeInfo Create(ResumeInfo model) {
            if (model.Id == 0)
            {
                int id = ResumeManage.Insert(model);
                model.Id = id;
            }
            else { 
                //Update
            }
            return model;
        }
        /// <summary>
        /// 根据GUID获取 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ResumeInfo GetByGUID(string guid) {
            return ResumeManage.GetByGUID(guid);
        }
         /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ResumeInfo> List(SearchSetting setting) {
            return ResumeManage.List(setting);
        }
        /// <summary>
        /// 删除（逻辑删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            return ResumeManage.Delete(id);
        }
    }
}
