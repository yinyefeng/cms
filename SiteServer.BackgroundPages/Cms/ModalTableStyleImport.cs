﻿using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStyleImport : BasePageCms
    {
		public HtmlInputFile HifMyFile;

        private string _tableName;
        private int _relatedIdentity;

        public static string GetOpenWindowString(string tableName, int publishmentSystemId, int relatedIdentity)
        {
            return LayerUtils.GetOpenScript("导入表样式",
                PageUtils.GetCmsUrl(nameof(ModalTableStyleImport), new NameValueCollection
                {
                    {"TableName", tableName},
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"RelatedIdentity", relatedIdentity.ToString()}
                }), 760, 200);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = Body.GetQueryString("TableName");
            _relatedIdentity = int.Parse(Body.GetQueryString("RelatedIdentity"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (HifMyFile.PostedFile != null && "" != HifMyFile.PostedFile.FileName)
			{
				var filePath = HifMyFile.PostedFile.FileName;
                if (!EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(filePath)))
				{
                    FailMessage("必须上传Zip压缩文件");
					return;
				}

				try
				{
                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                    HifMyFile.PostedFile.SaveAs(localFilePath);

                    ImportObject.ImportTableStyleByZipFile(_tableName, _relatedIdentity, localFilePath);

                    Body.AddSiteLog(PublishmentSystemId, "导入表单显示样式");

                    LayerUtils.Close(Page);
				}
				catch(Exception ex)
				{
					FailMessage(ex, "导入表样式失败！");
				}
			}
		}
	}
}
