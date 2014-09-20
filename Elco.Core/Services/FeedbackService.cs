using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class FeedbackService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static FeedbackInfo Create(FeedbackInfo model) { 
            if(model.Id == 0){
                int id = FeedbackManage.Insert(model);
                model.Id = id;
            }
            return model;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            return FeedbackManage.Delete(id);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<FeedbackInfo> List(SearchSetting setting) {
            return FeedbackManage.List(setting);
        }
    }
}
