@model IList<DoAnCNTT.Models.ApplicationUser>

@{
    ViewData["Title"] = "Danh sách nhân viên";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<h1>Danh sách nhân viên</h1>
<table class="table">
    <thead>
        <tr>
            <th>
                Nhân viên
            </th>
            <th>
                Email
            </th>
            <th>
                SĐT
            </th>
            <th>
                Ngày sinh
            </th>
            <th>
                Trạng thái
            </th>
            <th>
                Thao tác
            </th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>
                    <a asp-area="Admin" asp-controller="Employees" asp-action="Details" asp-route-id="@item.Id"><img class="user-img-home" src="@Url.Content(item.Image)" alt="Image" /> @item.Name</a>
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.Email)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.PhoneNumber)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.Birthday)
                </td>
                @if(item.LockoutEnd > DateTime.Now)
                {
                    <td>
                        Bị khóa
                    </td>
                    <td>
                        <a asp-area="Admin" asp-controller="Employees" asp-action="UnlockAccount" asp-route-userId="@item.Id">Mở khóa</a>
                    </td>
                }
                else
                {
                    <td>

                    </td>
                    <td>
                        <a asp-area="Admin" asp-controller="Employees" asp-action="LockAccount" asp-route-userId="@item.Id">Khóa tài khoản</a>
                    </td>
                }

            </tr>
        }
    </tbody>
</table>

