# automapper
2.x中使用automapper过于麻烦，所以简单封装下，支持直接通过autoInject(sourceType,tagettype,tagettype2)注解方式直接映射，也支持创建profile方式
两种使用方式参考：
        [Route("TestParamValide")]
        [HttpPost]
        public ActionResult<object> TestParamValide([FromBody]ValideModel model)
        {
            var sysDic = model.MapTo<SysDictionary>();
            return null;
        }

        [Route("TestParamValideMapperProfile")]
        [HttpPost]
        public ActionResult<object> TestParamValideMapperProfile([FromBody]ValideModel2 model)
        {
            var sysDic = model.MapTo<SysDictionary>();
            return null;
        }
        //方式一：直接使用AutoInject
         [AutoInject(typeof(SysDictionary), typeof(ValideModel))]
        public class ValideModel : Infrastructure.Mapper.DTO
        {
            [Required(ErrorMessage = "{0}不可为空")]
            public string Code { get; set; }
        }

        public class ValideModel2 : Infrastructure.Mapper.DTO
        {
            [Required(ErrorMessage = "{0}不可为空")]
            public string Code { get; set; }
        }
        //方式二：创建Profile
        public class ValideModelMapperProfile : Profile, IMapperProfile
        {
            public ValideModelMapperProfile()
            {
                CreateMap<SysDictionary, ValideModel2>().ReverseMap();
            }
        }
