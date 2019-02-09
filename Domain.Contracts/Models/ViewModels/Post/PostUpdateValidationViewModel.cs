using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Domain.Contracts.Models.ViewModels.Tag;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostUpdateValidationViewModel : IDataErrorInfo, INotifyDataErrorInfo
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public string Section { get; set; }
        
        public int UserProfileId { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public bool BelongsToUser { get; set; }

        public List<TagViewModel> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public bool IsFinished { get; set; }


        public string this[string property]
        {
            get
            {
                return GetErrors(property).Cast<string>().FirstOrDefault<string>();
            }
        }
        public string Error
        {
            get
            {
                return null;
            }
        }
        public bool HasErrors => GetErrors(null).Cast<string>().Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        public IEnumerable GetErrors(string property)
        {
            if (property == null || property == nameof(this.Header))
            {
                if (string.IsNullOrEmpty(this.Header))
                {
                    yield return $"{nameof(this.Header)} is mandatory";
                }
            }
            if (property == null || property == nameof(this.Content))
            {
                if (string.IsNullOrEmpty(this.Content))
                {
                    yield return $"{nameof(this.Content)} is mandatory";
                }
            }
        }
    }
}
