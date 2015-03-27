using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Empiria.WebAPI {

  public class LoginModel {

    [Required(ErrorMessage = "The api_key field is required")]
    public string api_key {
      get;
      set;
    }

    [Required(ErrorMessage = "The user_name field is required")]
    public string user_name {
      get;
      set;
    }

    [Required(ErrorMessage = "The password field is required")]
    public string password {
      get;
      set;
    }

  }  // class LoginModel
} // namespace Empiria.WebAPI
