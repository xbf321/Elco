using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class AttachmentService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AttachmentInfo Create(AttachmentInfo model) {
            if (model.Id == 0)
            {
                int id = AttachmentManage.Insert(model);
                model.Id = id;
            }
            else {
                AttachmentManage.Update(model);
            }
            return model;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            return AttachmentManage.Delete(id);
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AttachmentInfo Get(int id) {
            return AttachmentManage.Get(id);
        }
        /// <summary>
        /// 更新下载次数
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateDownloadCount(int id) {
            AttachmentManage.UpdateDownloadCount(id);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<AttachmentInfo> List(AttachmentSearchSetting setting)
        {
            return AttachmentManage.List(setting);
        }
        /// <summary>
        /// 根据GUID获得详细信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static AttachmentInfo GetByGUID(string guid) {
            return AttachmentManage.GetByGUID(guid);
        }
         /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<AttachmentDownloadLogInfo> ListLog(int attachId, SearchSetting setting) {
            return AttachmentManage.ListLog(attachId,setting);
        }

        /// <summary>
        /// 插入下载日志
        /// </summary>
        /// <param name="model"></param>
        public static void InsertLog(AttachmentDownloadLogInfo model) {
            AttachmentManage.InsertLog(model);
        }
    }
}
