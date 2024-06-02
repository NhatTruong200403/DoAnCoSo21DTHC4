@model ﻿DoAnCNTT.Models.ApplicationUser
@using Microsoft.AspNetCore.Identity
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

@{
    ViewData["Title"] = "Thông tin chủ xe";
    Layout = "/Views/Shared/_Layout.cshtml";
}
@{
    var listpost = ViewBag.listpost as List<DoAnCNTT.Models.Post>;

    var userBooking = ViewBag.userBooking as List<DoAnCNTT.Models.Booking>;
}



<div class="bigContai">
    <div style="display:flex;justify-content:space-between;background-color:antiquewhite;padding:15px 20px;">
        <div style="display:flex;">
            <a href="javascript:history.go(-1);" style="margin-right:0px;">
                <img style="width:30px;margin-top:2px;" src="@Url.Content("~/images/logo/muiten.png")" alt="" /><a>
            <h3>
                 @Model.Name
            </h3>
        </div>
        </div>
    <div style="display:flex;">
       @*  justify-content:space-between; *@
        <div>
            <img style="border-radius:50%;" src="@Url.Content(Model.Image)" alt="User Image" />
        </div>
        <div style="margin-left:20px;">
            <h2>Tên của chủ xe : @Model.Name</h2>
            <h4>Thông tin liên hệ </h4>    
            <p>Số điện thoại: @Model.PhoneNumber</p>
            <p hidden>Liên hệ chủ xe qua: 
                <a href="zalos://@Model.PhoneNumber">
                    <img style="width:30px;box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);" src="@Url.Content("~/images/logo/zalo.png")" alt="Zalo" />
                </a>
            </p>
            @if (User.IsInRole("Admin"))
            {
                <a class="nav-link den" asp-area="Admin" asp-controller="Customers" asp-action="GetBookingHistory" asp-route-userId="@Model.Id">Lịch sử đặt xe</a>
                <a class="nav-link den" asp-area="Admin" asp-controller="Customers" asp-action="GetPaymentHistory" asp-route-userId="@Model.Id">Lịch sử giao dịch</a>
            }
          
        </div>
    </div>
    <hr style="width:100%;"/>
    <div style="margin-left:10px;" >
        <h3 style="margin-left:10px;">Các bài viết khác của chủ xe</h3>
        <section class="py-2">
            <div class="container ">
                <div class="row gx-4 gx-lg-5 row-cols-2 row-cols-md-3 row-cols-xl-4 justify_content-center">

                    @if (listpost != null && listpost.Count > 0)
                    {
                        @foreach (var item in listpost)
                        {

                            <div class="box" style="padding:3px;margin-left:20px;background-color:white;">

                                <div class="pta" style="width: 280px;height:180px; overflow: hidden;">
                                    <img src="@item.Image" alt="" style="object-fit: cover; width: 100%; height: 100%;" />
                                </div>

                                <div class="ptb">
                                    @if (item.Gear)
                                    {
                                        <div class="ptb1 apt" style="padding:2px 4px;">Số tự động</div>
                                    }
                                    else
                                    {
                                        <div class="ptb1 apt" style="padding:2px 4px;">Số sàn</div>
                                    }
                                    <div class="ptb1 bpt" style="padding:2px 4px;">Giao xe tận nơi</div>
                                    <div class="ptb1 cpt" style="padding:2px 4px;">Giảm 20%</div>
                                </div>

                        <div class="ptc"><a asp-action="Details" asp-route-id="@item.Id">@Html.DisplayFor(modelItem => item.Name)</a></div>
                        <div class="ptd">
                            <img src="@Url.Content("~/images/logo/road-map-line.svg")" class="icon_map" alt="">
                            <span class="text_map">@Html.DisplayFor(modelItem => item.RentLocation)</span>
                        </div>
                        <hr>
                        <div class="pte">
                            <div class="pte_left">
                                <div class="dgsao">
                                    <img src="@Url.Content("~/images/logo/star-s-fill.svg")" class="icon_sao_danhgia" alt="">
                                    <span class="text_saodanhgia">@Html.DisplayFor(modelItem => item.AvgRating)</span>
                                </div>

                                        <div class="sochuyen">
                                            <img src="@Url.Content("~/images/logo/luggage-cart-line.svg")" class="icon_map" alt="">
                                            <div class="text_sochuyen">20 chuyến</div>
                                        </div>
                                        <!-- <div class="sochuyen">
                                            Chưa có chuyến.
                                        </div> -->
                                    </div>
                                    <div class="pte_right">
                                        @* <div class="giagoc">100k</div> *@
                                        <div class="giagiam">@Html.DisplayFor(modelItem => item.Price) /ngày</div>
                                    </div>
                                </div>

                            </div>
                        }
                    }
                    else
                    {
                        <span>
                            Chủ xe không có bài viết khác
                        </span>
                    }
                </div>
            </div>
    </div>
    <div>
        <a class="nav-link den" asp-area="Admin" asp-controller="Customers" asp-action="LockAccount" asp-route-userId="@Model.Id">Khóa tài khoản</a>
    </div>
    </section>
</div>

<style>
    .bigContai {
        border-radius:5px;
        width: 85%;
        background-color: white;
        margin: auto;
        @* padding: 25px; *@
    }

    .box {
        width: 305px;
        height: 345px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        border-radius: 15px;
    }

    .pta {
        padding: 0px;
        margin: 10px;
    }

        .pta img {
            border-radius: 15px;
            width: 280px;
            height: 180px;
        }

    .ptb {
        padding: 0px;
        display: flex;
        width: 100%;
        margin: 0;
    }

    .ptb1 {
        font-size: 13px;
        border-radius: 50px;
        margin: 0;
    }

        .ptb1.apt {
            background-color: aquamarine;
        }

        .ptb1.bpt {
            background-color: rgb(243, 253, 49);
        }

        .ptb1.cpt {
            background-color: rgb(79, 143, 255);
        }

    .ptb div {
        padding: 0px;
        margin-inline: 10px;
    }

    .ptc {
        padding: 0px;
        margin-block: 7px;
    }

    .ptd {
        padding: 0px;
        margin-inline: 10px;
        display: flex;
    }

        .ptd .icon_map {
            width: 25px;
        }

        .ptd .text_map {
            padding-top: 3px;
        }

    hr {
        width: 275px;
        margin-top: 10px;
        margin-bottom: 5px;
    }

    .pte {
        padding: 0px;
        margin-inline: 10px;
        display: flex;
        justify-content: space-between;
    }

        .pte .pte_left {
            display: flex;
        }

            .pte .pte_left .dgsao {
                display: flex;
                margin-right: 10px;
            }

            .pte .pte_left .sochuyen {
                display: flex;
            }

                .pte .pte_left .sochuyen .icon_map {
                    width: 19px;
                }

            .pte .pte_left .dgsao .icon_sao_danhgia {
                width: 18px;
                /* color: rgb(253, 236, 0); */
            }

        .pte .pte_right {
            display: flex;
        }

            .pte .pte_right div {
                margin-right: 10px;
            }

</style>