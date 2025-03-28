﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Common.Classes
{
    public abstract class BaseConverter<TSourceObj, TDestinationObj> : IBaseConverter<TSourceObj, TDestinationObj>
      where TSourceObj : class
      where TDestinationObj : class
    {
        //Any derived class needs this to convert for a single object
        public abstract TDestinationObj ConvertObject(TSourceObj srcObj);

        //Convert collection of source object to destination object
        public virtual List<TDestinationObj> ConvertObjectCollection(IEnumerable<TSourceObj> srcObjList)
        {
            if (srcObjList == null) return null;
            var destList = srcObjList.Select(item => this.ConvertObject(item));
            return destList.ToList();
        }
    }
}
