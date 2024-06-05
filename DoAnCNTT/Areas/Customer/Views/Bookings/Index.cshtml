@model IEnumerable<DoAnCNTT.Models.Booking>
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@{
    ViewData["Title"] = "Lịch sử đặt xe";
    Layout = "~/Views/Shared/_Layout.cshtml";

}

<h1>Lịch sử đặt xe</h1>

<div>

        @foreach (var item in Model)
        {
                var currentUser = await UserManager.FindByIdAsync(item.Post.UserId!);
                <div style="box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);margin:10px;padding:30px;margin-bottom:20px;background-color:white;border-radius:5px;">
                    
                    <div class="tren">
                        <div class="trai">Tên chủ xe : @currentUser.Name</div>
                        <div class="phai" style="color:orange;font-size:20px;">@item.Status</div>
                    </div>
                    <hr />
                    <div class="giua">
                        <div class="trai" style="display:flex;">
                            <img src="@Url.Content(item.Post.Image)" style="height:150px;box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);border-radius:5px;" />
                            <span style="padding:10px 20px;font-size:23px;font-weight:bold;">@item.Post.Name</span>
                        </div>
                        <div class="phai">
                            <p>Số tiền gốc : @item.Post.Price/giờ</p>
                            <p>Thành tiền : @item.Total</p>
                        </div>
                    </div>
                    <hr />
                    <div class="duoi">
                        <div class="trai">
                            <a asp-action="Details" asp-route-id="@item.Id" style="padding:13px 20px;background-color:cadetblue;border-radius:5px;">Xem chi tiết</a>
                            @if(@item.RecieveOn <= @DateTime.Now)
                            {
                                <a asp-action="Delete" hidden asp-route-id="@item.Id" style="padding:13px 20px;background-color:cadetblue;border-radius:5px;">Yêu cầu trả cọc</a>
                            }
                            else
                            {
                            <a asp-action="Delete" asp-route-id="@item.Id" style="padding:13px 20px;background-color:cadetblue;border-radius:5px;">Yêu cầu trả cọc</a>
                            }
                        </div>
                        <div class="phai">
                            <div>
                                    Đã đặt cọc : @item.PrePayment
                                </div>
                            </div>
                    </div>
                </div>
        }
</div>
@* 
<table class="table">
    <thead>
        <tr>
            <th>
                Bài đăng
            </th>
            <th>
                Ngày nhận
            </th>
            <th>
                Ngày trả
            </th>
            <th>
                Tổng tiền
            </th>
            <th>
                Thành tiền
            </th>
            <th>
                Đặt cọc
            </th>
            <th>
                Trạng thái
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                <a asp-action="Details" asp-route-id="@item.PostId">@item.Post.Name</a>
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.RecieveOn)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ReturnOn)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Total)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.FinalValue)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.PrePayment)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Status)
            </td>
            <td>
                <a asp-action="Details" asp-route-id="@item.Id">Chi tiết đặt cọc</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Yêu cầu trả cọc</a>
            </td>
        </tr>
}
    </tbody>
</table>
 *@
<style>
    .tren,.giua,.duoi{
        display: flex;
        justify-content: space-between;
        align-items: center;

    }

</style>

    @* .duoi {
        display: flex;
        justify-content: flex-end;
    }  *@