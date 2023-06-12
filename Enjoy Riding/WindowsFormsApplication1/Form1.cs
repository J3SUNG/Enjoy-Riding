using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using Oracle.DataAccess.Client;
using System.IO;
 
namespace Enjoy_Riding
{
    public partial class Form1 : Form
    {
        bool idFlag; // 중복검사
        int rentDuring = 0;
        string id = "";
        string grade;
        string chartWhere;
        string chartWhere2;
        string[] filter = new string[8];
        string[] stateArr = new string[5] { "매우 나쁨", "나쁨", "보통", "좋음", "매우 좋음" };

        public Form1()
        {
            InitializeComponent();
        }

        void send_mail(string r_mail, string name)
        {
            const string SMTP_SERVER = "smtp.naver.com"; // SMTP 서버 주소
            const int SMTP_PORT = 587; // SMTP 포트
            const string MAIL_ID = "wptjd6141@naver.com"; // 보내는 사람의 이메일
            const string MAIL_ID_NAME = "wptjd6141"; // 보내는사람 계정 ( 네이버 로그인 아이디 ) 
            const string MAIL_PW = "!Xungl440*'";  // 보내는사람 패스워드 ( 네이버 로그인 패스워드 )

            try
            {
                MailAddress mailFrom = new MailAddress(MAIL_ID, MAIL_ID_NAME, Encoding.UTF8); // 보내는사람의 정보를 생성
                MailAddress mailTo = new MailAddress(r_mail); // 받는사람의 정보를 생성
                SmtpClient client = new SmtpClient(SMTP_SERVER, SMTP_PORT); // smtp 서버 정보를 생성
                MailMessage message = new MailMessage(mailFrom, mailTo);

                message.Subject = "렌트하신 차량을 반납해주시기 바랍니다."; // 메일 제목 프로퍼티
                message.Body = "안녕하세요. 렌트샵입니다. \n " + name + "님이 대여하신 차량의 렌트기간이 지났습니다. 서둘러 반납해주시기 바랍니다."; // 메일의 몸체 메세지 프로퍼티
                message.BodyEncoding = Encoding.UTF8; // 메세지 인코딩 형식
                message.SubjectEncoding = Encoding.UTF8; // 제목 인코딩 형식

                client.EnableSsl = true; // SSL 사용 유무 (네이버는 SSL을 사용합니다. )
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential(MAIL_ID, MAIL_PW); // 보안인증 ( 로그인 )
                client.Send(message);  //메일 전송 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }   // 메일 전송

        void fill()
        {
            try
            {
                pEOPLETableAdapter.Fill(dataSet1.PEOPLE);
                rENTTableAdapter.Fill(dataSet1.RENT);
                rEVIEWTableAdapter.Fill(dataSet1.REVIEW);
                v_TYPETableAdapter.Fill(dataSet1.V_TYPE);
                vEHICLE_DETAILTableAdapter.Fill(dataSet1.VEHICLE_DETAIL);
                vEHICLETableAdapter.Fill(dataSet1.VEHICLE);
                vehicleTableAdapter1.Fill(dataSet1.VEHICLE);
                vIEW_RENTTableAdapter.Fill(dataSet1.VIEW_RENT);
                vIEW_RESERVETableAdapter.Fill(dataSet1.VIEW_RESERVE);
                vIEW_VEHICLETableAdapter.Fill(dataSet1.VIEW_VEHICLE);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void panel_close()
        {
            fill();
            panel1.Visible = false; // 메인
            panel2.Visible = false; // 회원가입
            panel3.Visible = false; // 로그인
            panel4.Visible = false; // 내정보
            panel5.Visible = false; // 렌트
            panel6.Visible = false; // 유저관리
            panel7.Visible = false; // 차량관리
            panel8.Visible = false; // 리뷰관리
            panel9.Visible = false; // 통계
            panel10.Visible = false; // 차량 보기
            panel11.Visible = false; // 렌트 기록
        }   // 패널 변경 함수

        int category_load(string s)
        {
            int index = 0;

            switch (s)
            {
                case "전용기":
                    index = 1;
                    break;
                case "버스":
                    index = 2;
                    break;
                case "대형차":
                    index = 3;
                    break;
                case "중형차":
                    index = 4;
                    break;
                case "소형차":
                    index = 5;
                    break;
                case "전기차":
                    index = 6;
                    break;
                case "오토바이":
                    index = 7;
                    break;
                case "스쿠터":
                    index = 8;
                    break;
                case "전동킥보드":
                    index = 9;
                    break;
                case "전기자전거":
                    index = 10;
                    break;
                default:
                    index = 0;
                    break;
            }

            return index;
        }   // 카테고리 번호 출력

        string category_load(int i)
        {
            string s = "";

            switch (i)
            {
                case 1:
                    s = "전용기";
                    break;
                case 2:
                    s = "버스";
                    break;
                case 3:
                    s = "대형차";
                    break;
                case 4:
                    s = "중형차";
                    break;
                case 5:
                    s = "소형차";
                    break;
                case 6:
                    s = "전기차";
                    break;
                case 7:
                    s = "오토바이";
                    break;
                case 8:
                    s = "스쿠터";
                    break;
                case 9:
                    s = "전동킥보드";
                    break;
                case 10:
                    s = "전기자전거";
                    break;
                default:
                    s = "";
                    break;
            }

            return s;
        }   // 카테고리 타입 출력

        void filter_change()
        {
            try
            {
                string s;
                s = "MODEL LIKE '%" + filter[0] + "%'";
                if (filter[1] != "x")
                {
                    s += " AND V_NO = " + Convert.ToInt32(filter[1]);
                }

                s += " AND PRICE >= " + Convert.ToInt32(filter[2]);
                s += " AND PRICE <= " + Convert.ToInt32(filter[3]);

                if (filter[4] == "0")
                {
                    s += " AND RENT_STATE = '대기중'";
                }
                if (filter[5] != "x")
                {
                    s += " AND COLOR = '" + filter[5] + "'";
                }
                if (filter[6] != "x")
                {
                    s += " AND STATE = '" + filter[6] + "'";
                }
                if (filter[7] != "x")
                {
                    s += " AND LOC = '" + filter[7] + "'";
                }
                vIEWVEHICLEBindingSource.Filter = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }   // 렌트 : 필터 변경 함수

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.rENTTableAdapter.Fill(this.dataSet1.RENT);
                // TODO: 이 코드는 데이터를 'dataSet1.VIEW_RESERVE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vIEW_RESERVETableAdapter.Fill(this.dataSet1.VIEW_RESERVE);
                // TODO: 이 코드는 데이터를 'dataSet1.REVIEW' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.rEVIEWTableAdapter.Fill(this.dataSet1.REVIEW);
                // TODO: 이 코드는 데이터를 'dataSet13.VIEW_RESERVE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vIEW_RESERVETableAdapter.Fill(this.dataSet13.VIEW_RESERVE);
                // TODO: 이 코드는 데이터를 'dataSet1.VEHICLE_DETAIL' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vEHICLE_DETAILTableAdapter.Fill(this.dataSet1.VEHICLE_DETAIL);
                // TODO: 이 코드는 데이터를 'dataSet12.VEHICLE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vEHICLETableAdapter.Fill(this.dataSet12.VEHICLE);
                // TODO: 이 코드는 데이터를 'dataSet12.V_TYPE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.v_TYPETableAdapter.Fill(this.dataSet12.V_TYPE);
                // TODO: 이 코드는 데이터를 'dataSet1.VIEW_RESERVE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vIEW_RESERVETableAdapter.Fill(this.dataSet1.VIEW_RESERVE);
                // TODO: 이 코드는 데이터를 'dataSet1.VIEW_RENT' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vIEW_RENTTableAdapter.Fill(this.dataSet1.VIEW_RENT);
                // TODO: 이 코드는 데이터를 'dataSet1.PEOPLE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.pEOPLETableAdapter.Fill(this.dataSet1.PEOPLE);
                // TODO: 이 코드는 데이터를 'dataSet1.VIEW_VEHICLE' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
                this.vIEW_VEHICLETableAdapter.Fill(this.dataSet1.VIEW_VEHICLE);

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView6.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView7.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView8.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView9.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView10.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView11.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView12.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                oracleConnection1.Open();
                oracleDataAdapter1.Fill(dataSet11, "PEOPLE");
                oracleConnection1.Close();

                panel_close();
                panel1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)  // 메인 : 로그인
        {
            try
            {
                DataTable tempTable = dataSet11.Tables["PEOPLE"];
                DataRow tempRow = tempTable.Rows.Find(textBox1.Text);
                id = textBox1.Text;
                string pw = textBox2.Text;
                string blacklist;

                textBox1.Text = "";
                textBox2.Text = "";

                if (tempRow == null)
                {
                    MessageBox.Show("없는 ID 입니다.");
                }
                else
                {
                    if (tempRow["PW"].ToString().ToUpper().Equals(pw.ToUpper()))
                    {
                        blacklist = tempRow["BLACKLIST"].ToString();

                        if (blacklist == "10")
                        {
                            MessageBox.Show("블랙리스트 상태입니다. 예약을 할 수 없으며, 즉시 렌트만 가능합니다. \n관련 사항은 매니저에게 문의해주세요");
                        }
                        panel_close();
                        panel3.Visible = true;
                        grade = tempRow["GRADE"].ToString();

                        if (grade == "고객")
                        {
                            button7.Text = "내 정보";
                            button16.Visible = false;
                            button22.Visible = false;
                            button23.Visible = false;
                            button33.Visible = false;
                            button29.Visible = false;
                            label5.Visible = false;
                            label31.Visible = false;
                            comboBox7.Visible = false;
                            textBox21.Visible = false;
                            button44.Visible = false;
                            button48.Visible = false;
                        }
                        else if (grade == "매니저")
                        {
                            button7.Text = "렌트 정보";
                            button16.Visible = true;
                            button22.Visible = true;
                            button23.Visible = false;
                            button33.Visible = false;
                            button29.Visible = false;
                            label5.Visible = true;
                            label31.Visible = true;
                            comboBox7.Visible = true;
                            textBox21.Visible = true;
                            button44.Visible = true;
                            button48.Visible = false;
                        }
                        else if (grade == "관리자")
                        {
                            button7.Text = "렌트 정보";
                            button16.Visible = true;
                            button22.Visible = true;
                            button23.Visible = true;
                            button29.Visible = true;
                            button33.Visible = true;
                            label5.Visible = true;
                            label31.Visible = true;
                            comboBox7.Visible = true;
                            textBox21.Visible = true;
                            button44.Visible = true;
                            button48.Visible = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("비밀번호가 일치하지 않습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)  // 메인 : 회원가입
        {
            try
            {
                panel_close();
                panel2.Visible = true;
                idFlag = false;

                textBox1.Text = "";
                textBox2.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)  // 회원가입 : 중복검사
        {
            try
            {
                DataTable tempTable = dataSet11.Tables["PEOPLE"];
                id = textBox3.Text;

                if (id == "")
                {
                    MessageBox.Show("ID를 입력하세요");
                }
                else if (id.Length < 4 || id.Length > 12)
                {
                    MessageBox.Show("ID는 4 ~ 12자리로 입력해주세요.");
                }
                else
                {
                    foreach (DataRow tempRow in tempTable.Rows)
                    {
                        if (tempRow["ID"].ToString().ToLower().Equals(id.ToLower()))
                        {
                            idFlag = false;
                            break;
                        }
                        else
                        {
                            idFlag = true;
                        }
                    }

                    if (idFlag)
                    {
                        MessageBox.Show("사용 가능한 ID입니다.");
                    }
                    else
                    {
                        MessageBox.Show("이미 사용중인 ID입니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)  // 회원가입 : 회원가입
        {
            try
            {
                bool flag = false;
                string id = textBox3.Text;
                string pw1 = textBox4.Text;
                string pw2 = textBox5.Text;
                string name = textBox6.Text;
                string phone = textBox7.Text;
                string inputCode = textBox8.Text;
                string email = textBox32.Text;
                string code = "1234";

                for (int i = 0; i < id.Length; ++i)
                {
                    if ((id[i] >= 'a' && id[i] <= 'z') || (id[i] >= '0' && id[i] <= '9'))
                    {
                        continue;
                    }
                    else
                    {
                        flag = true;
                        MessageBox.Show("소문자와 숫자만 입력해주세요");
                        break;
                    }
                }
                if (!flag)
                {
                    if (id == "")
                    {
                        MessageBox.Show("ID를 입력하세요");
                    }
                    else if (id.Length < 4 || id.Length > 12)
                    {
                        MessageBox.Show("ID는 4 ~ 12자리로 입력해주세요.");
                    }
                    else if (pw1 == "" || pw2 == "")
                    {
                        MessageBox.Show("PW를 입력하세요");
                    }
                    else if (pw1.Length < 4 || pw1.Length > 12)
                    {
                        MessageBox.Show("PW는 4 ~ 12자리로 입력해주세요.");
                    }
                    else if (pw1 != pw2)
                    {
                        MessageBox.Show("비밀번호가 일치하지 않습니다.");
                    }
                    else if (name == "")
                    {
                        MessageBox.Show("이름를 입력하세요.");
                    }
                    else if (phone == "")
                    {
                        MessageBox.Show("전화번호를 입력하세요.");
                    }
                    else if (email == "")
                    {
                        MessageBox.Show("이메일을 입력하세요.");
                    }
                    else if (!idFlag)
                    {
                        MessageBox.Show("중복검사를 해주세요.");
                    }
                    else if (checkBox1.Checked && inputCode != code)
                    {
                        MessageBox.Show("code가 일치하지 않습니다.");
                    }
                    else
                    {
                        DataTable tempTable = dataSet1.Tables["PEOPLE"];
                        DataRow myNewDataRow = tempTable.NewRow();

                        myNewDataRow["ID"] = id;
                        myNewDataRow["PW"] = pw1;
                        myNewDataRow["NAME"] = name;
                        myNewDataRow["PHONE"] = phone;
                        myNewDataRow["BLACKLIST"] = "0";
                        myNewDataRow["USED_FEE"] = 0;
                        myNewDataRow["RENT_COUNT"] = "0";
                        myNewDataRow["LATE_FEE"] = 0;

                        if (checkBox1.Checked && inputCode == code)
                        {
                            MessageBox.Show("매니저 " + name + "님 회원가입 완료!");
                            myNewDataRow["GRADE"] = "매니저";
                        }
                        else
                        {
                            MessageBox.Show(name + "님 회원가입 완료!");
                            textBox3.Text = "";
                            textBox4.Text = "";
                            textBox5.Text = "";
                            textBox6.Text = "";
                            textBox7.Text = "";
                            textBox8.Text = "";
                            textBox32.Text = "";
                            checkBox1.Checked = false;
                            myNewDataRow["GRADE"] = "고객";
                        }

                        tempTable.Rows.Add(myNewDataRow);
                        oracleDataAdapter1.Update(dataSet1.PEOPLE);

                        oracleConnection1.Open();
                        oracleCommand1.CommandText = "UPDATE PEOPLE SET EMAIL = '" + email + "' WHERE ID = '" + id + "'";
                        oracleCommand1.ExecuteNonQuery();
                        oracleConnection1.Close();

                        pEOPLETableAdapter.Fill(dataSet11.PEOPLE);

                        panel_close();
                        panel1.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)  // 회원가입 : 뒤로가기
        {
            try
            {
                panel_close();
                panel1.Visible = true;

                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox32.Text = "";
                checkBox1.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)   // 회원가입 : 관리자 회원가입
        {
            try
            {
                if (checkBox1.Checked)
                {
                    textBox8.Text = "";
                    textBox8.Visible = true;
                }
                else
                {
                    textBox8.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)  // 로그인 : 렌트
        {
            try
            {
                for (int i = 0; i < 8; ++i)
                {
                    filter[i] = "x";
                }
                filter[0] = "";
                filter[2] = "0";
                filter[3] = "100000";
                filter[4] = "-1";

                panel_close();
                panel5.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)  // 로그인 : 내 정보
        {
            try
            {
                panel_close();
                panel4.Visible = true;

                textBox23.Text = "";
                textBox24.Text = "";
                textBox25.Text = "";
                textBox26.Text = "";
                checkBox4.Checked = false;

                if (grade == "고객")
                {
                    label28.Visible = false;
                    label29.Visible = false;
                    textBox14.Visible = false;
                    textBox15.Visible = false;

                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중' AND ID = '" + id + "'";
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중' AND ID = '" + id + "'";
                    rEVIEWBindingSource1.Filter = "id = '" + id + "' AND CONTENT IS NULL";
                    rENTBindingSource1.Filter = "id = '" + id + "' AND RESULT = '신청중'";
                }
                else
                {
                    label28.Visible = true;
                    label29.Visible = true;
                    textBox14.Visible = true;
                    textBox15.Visible = true;
                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중'";
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중'";
                    rEVIEWBindingSource1.Filter = "CONTENT IS NULL";
                    rENTBindingSource1.Filter = "RESULT = '신청중'";
                }

                oracleConnection1.Open();
                DataTable tempTable = dataSet1.Tables["PEOPLE"];
                DataRow tempRow = tempTable.Rows.Find(id);
                oracleConnection1.Close();

                label13.Text = "ID : " + id;
                label14.Text = "이름 : " + tempRow["NAME"];
                label15.Text = "전화번호 : " + tempRow["PHONE"];
                label16.Text = "직급 : " + grade;
                label17.Text = "빌린횟수 : " + tempRow["RENT_COUNT"] + "번 대여했습니다.";
                label18.Text = "연체료 : " + tempRow["late_fee"] + "원";
                label19.Text = "블랙리스트 경고 수 : " + tempRow["blacklist"] + "회 (10번 = 블랙리스트)";
                label2.Text = "사용금액 : " + tempRow["used_fee"] + "원";
                label3.Text = "이메일 : " + tempRow["email"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button16_Click(object sender, EventArgs e) // 로그인 : 유저관리
        {
            try
            {
                panel_close();
                panel6.Visible = true;

                pEOPLETableAdapter.Fill(dataSet1.PEOPLE);
                if (grade == "매니저")
                {
                    pEOPLEBindingSource.Filter = "GRADE = '고객'";
                }
                else
                {
                    pEOPLEBindingSource.RemoveFilter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)  // 로그인 : 로그아웃
        {
            try
            {
                panel_close();
                panel1.Visible = true;
                id = "";
                grade = "고객";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e) // 내정보-렌트정보 : 검색
        {
            try
            {
                if (grade == "고객")
                {
                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중' AND ID = '" + id + "' AND MODEL LIKE '%" + textBox9.Text + "%'";
                }
                else
                {
                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중' AND ID LIKE '%" + textBox14.Text + "%' AND MODEL LIKE '%" + textBox9.Text + "%'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button11_Click(object sender, EventArgs e) // 내정보-렌트정보 : 원래대로
        {
            try
            {
                textBox9.Text = "";
                if (grade == "고객")
                {
                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중' AND ID = '" + id + "'";
                }
                else
                {
                    vIEWRENTBindingSource.Filter = "RESULT = '렌트중'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button12_Click(object sender, EventArgs e) // 내정보-예약정보 : 검색
        {
            try
            {
                if (grade == "고객")
                {
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중' AND ID = '" + id + "'";
                }
                else
                {
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중'";
                }

                if (grade == "고객")
                {
                    vIEWRESERVEBindingSource.Filter = "ID = '" + id + "' AND MODEL LIKE '%" + textBox10.Text + "%' AND RESULT = '예약중'";
                }
                else
                {
                    vIEWRESERVEBindingSource.Filter = "ID LIKE '%" + textBox15.Text + "%' AND MODEL LIKE '%" + textBox10.Text + "%' AND RESULT = '예약중'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button13_Click(object sender, EventArgs e) // 내정보-예약정보 : 원래대로
        {
            try
            {
                textBox9.Text = "";

                if (grade == "고객")
                {
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중' AND ID = '" + id + "'";
                }
                else
                {
                    vIEWRESERVEBindingSource.Filter = "RESULT = '예약중'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)  // 내정보 : 뒤로가기
        {
            try
            {
                textBox9.Text = "";
                panel_close();
                panel3.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)   // 렌트 : 품절
        {
            try
            {
                if (checkBox2.Checked) // 필터
                {
                    filter[4] = "0";
                }
                else // 필터
                {
                    filter[4] = "-1";
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button15_Click(object sender, EventArgs e) // 렌트 : 렌트
        {
            try
            {
                int count;
                DateTime date;
                int index = dataGridView1.CurrentCell.RowIndex;
                string model = dataGridView1.Rows[index].Cells[0].Value.ToString();
                string vehicle_no = dataGridView1.Rows[index].Cells["VEHICLE_NO"].Value.ToString();
                string detail_no = dataGridView1.Rows[index].Cells["DETAIL_NO"].Value.ToString();
                string rent_state = dataGridView1.Rows[index].Cells["RENT_STATE"].Value.ToString();
                string used_fee = (Convert.ToInt32(dataGridView1.Rows[index].Cells[5].Value) * rentDuring).ToString();

                if (rentDuring == 0)
                {
                    MessageBox.Show("대여 날짜를 선택해 주세요.");
                }
                else
                {
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT USED_FEE FROM PEOPLE WHERE ID = '" + id + "'";
                    int cur_fee = Convert.ToInt32(oracleCommand1.ExecuteScalar()) + Convert.ToInt32(used_fee);

                    if (rent_state == "신청중")
                    {
                        MessageBox.Show("신청 중인 차량은 렌트할 수 없습니다. 매니저에게 문의해주세요.");
                    }
                    else if (rent_state == "대기중")
                    {
                        if (MessageBox.Show(model + "를 " + rentDuring + "일 동안 렌트신청합니다. " + used_fee + "원 입니다.", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DataTable tempTable = dataSet11.Tables["RENT"];
                            DataRow myNewDataRow = tempTable.NewRow();

                            oracleCommand1.CommandText = "SELECT RENT_SEQ.nextval FROM DUAL";
                            count = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                            oracleCommand1.CommandText = "SELECT SYSDATE FROM DUAL";
                            date = Convert.ToDateTime(oracleCommand1.ExecuteScalar());

                            string date1 = date.ToString("yyyyMMdd");
                            string date2 = date.AddDays(rentDuring).ToString("yyyyMMdd");

                            oracleCommand1.CommandText = "INSERT INTO RENT VALUES (" + count + ",'" + id + "', " + vehicle_no + ", TO_DATE('" + date1 + "','yyyymmdd'), " + "TO_DATE('" + date2 + "','yyyymmdd')" + ", '" + model + "', " + detail_no + ", '신청중', " + used_fee + ", ' ', ' ')";    // 추가
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE PEOPLE SET RENT_COUNT = RENT_COUNT + 1 WHERE ID = '" + id + "'";
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '신청중' WHERE DETAIL_NO = " + detail_no;
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = " + cur_fee + " WHERE ID = '" + id + "'";
                            oracleCommand1.ExecuteNonQuery();

                            MessageBox.Show("렌트 신청 완료");
                        }
                    }
                    else if (rent_state == "렌트중")
                    {
                        oracleCommand1.CommandText = "SELECT RENT_END FROM RENT WHERE DETAIL_NO = " + detail_no;
                        DateTime rent_end = Convert.ToDateTime(oracleCommand1.ExecuteScalar());

                        oracleCommand1.CommandText = "SELECT BLACKLIST FROM PEOPLE WHERE ID = '" + id + "'";
                        string blacklist = oracleCommand1.ExecuteScalar().ToString();

                        if (blacklist == "10")
                        {
                            MessageBox.Show("블랙리스트 상태에서는 예약이 불가능합니다. 즉시 렌트만 가능하며, \n블랙리스트 해제는 매니저에게 문의해주세요.");
                        }
                        else if (MessageBox.Show(model + "는 품절입니다. 예약하시겠습니까? " + used_fee + "원 입니다.", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DataTable tempTable = dataSet11.Tables["RESERVE"];
                            DataRow myNewDataRow = tempTable.NewRow();

                            oracleCommand1.CommandText = "SELECT RESERVE_SEQ.nextval FROM DUAL";
                            count = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                            oracleCommand1.CommandText = "INSERT INTO RESERVE VALUES (" + count + ",'" + id + "', " + vehicle_no + ", '" + rentDuring + "', '" + model + "', " + detail_no + ", '예약중', " + used_fee + ")";
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '예약중' WHERE DETAIL_NO = " + detail_no;
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = " + cur_fee + " WHERE ID = '" + id + "'";
                            oracleCommand1.ExecuteNonQuery();

                            MessageBox.Show("예약 되었습니다.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("품절 상태이며, 다른 사람이 예약 중 입니다.");
                    }

                    fill();
                    oracleConnection1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button17_Click(object sender, EventArgs e) // 렌트 : 뒤로가기
        {
            try
            {
                panel_close();
                panel3.Visible = true;

                rentDuring = 0;
                textBox11.Text = "";
                textBox12.Text = "";
                textBox13.Text = "";
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
                checkBox2.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button18_Click(object sender, EventArgs e) // 유저관리 : 뒤로가기
        {
            try
            {
                panel_close();
                panel3.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) // 렌트 : 필터 - 분류
        {
            try
            {
                int index = category_load(comboBox1.Text);

                if (index == 0) // 필터 해제
                {
                    filter[1] = "x";
                }
                else // 필터
                {
                    filter[1] = index.ToString();
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)  // 렌트 : 가격 시작점
        {
            try
            {
                bool flag = false;
                string s = textBox11.Text;

                if (s.Length > 5)
                {
                    MessageBox.Show("가격은 5자리 이하를 입력해주세요.");
                    filter[2] = "0";
                    textBox11.Text = "";
                }
                else
                {
                    for (int i = 0; i < s.Length; ++i)
                    {
                        if (s[i] < '0' || s[i] > '9')
                        {
                            MessageBox.Show("숫자만 입력해주세요.");
                            textBox11.Text = "";
                            filter[2] = "0";
                            flag = true;
                            break;
                        }
                    }

                    if (s.Length == 0)
                    {
                        filter[2] = "0";
                        flag = true;
                    }

                    if (!flag)
                    {
                        filter[2] = textBox11.Text;
                        int num = Convert.ToInt32(textBox11.Text);
                    }
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) // 렌트 : 필터 - 색
        {
            try
            {
                filter[5] = comboBox2.Text;
                if (comboBox2.Text == "") // 필터 해제
                {
                    filter[5] = "x";
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) // 렌트 : 필터 - 상태
        {
            try
            {
                filter[6] = comboBox3.Text;
                if (comboBox3.Text == "") // 필터 해제
                {
                    filter[6] = "x";
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e) // 렌트 : 가격 끝점
        {
            try
            {
                bool flag = false;
                string s = textBox12.Text;

                if (s.Length > 5)
                {
                    MessageBox.Show("가격은 5자리 이하를 입력해주세요.");
                    filter[3] = "0";
                    textBox12.Text = "";
                }
                else
                {
                    for (int i = 0; i < s.Length; ++i)
                    {
                        if (s[i] < '0' || s[i] > '9')
                        {
                            MessageBox.Show("숫자만 입력해주세요.");
                            textBox12.Text = "";
                            filter[3] = "0";
                            flag = true;
                            break;
                        }
                    }

                    if (s.Length == 0)
                    {
                        filter[3] = "100000";
                        flag = true;
                    }

                    if (!flag)
                    {
                        filter[3] = textBox12.Text;
                        int num = Convert.ToInt32(textBox12.Text);
                    }
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)  // 렌트 : 모델명
        {
            try
            {
                filter[0] = textBox13.Text;
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e) // 렌트 : 대여날짜
        {
            try
            {
                string s = comboBox4.Text;
                if (s == "")
                {
                    rentDuring = 0;
                }
                else
                {
                    rentDuring = s[0] - 48;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button14_Click(object sender, EventArgs e) // 유저관리 : 수정완료
        {
            try
            {
                int index = dataGridView4.CurrentCell.RowIndex;
                string id = dataGridView4.Rows[index].Cells[0].Value.ToString();
                string pw = textBox23.Text;
                string name = textBox24.Text;
                string phone = textBox25.Text;

                oracleConnection1.Open();
                if (pw != "")
                {
                    oracleCommand1.CommandText = "UPDATE PEOPLE SET PW = '" + pw + "' WHERE ID = '" + id + "'";
                    oracleCommand1.ExecuteNonQuery();
                }
                if (name != "")
                {
                    oracleCommand1.CommandText = "UPDATE PEOPLE SET NAME = '" + name + "' WHERE ID = '" + id + "'";
                    oracleCommand1.ExecuteNonQuery();
                }
                if (phone != "")
                {
                    oracleCommand1.CommandText = "UPDATE PEOPLE SET PHONE = '" + phone + "' WHERE ID = '" + id + "'";
                    oracleCommand1.ExecuteNonQuery();
                }
                oracleConnection1.Close();

                textBox23.Text = "";
                textBox24.Text = "";
                textBox25.Text = "";

                pEOPLETableAdapter.Fill(dataSet1.PEOPLE);
                MessageBox.Show("등록 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button19_Click(object sender, EventArgs e) // 유저관리 : 삭제
        {
            try
            {
                int index = dataGridView4.CurrentCell.RowIndex;
                string s = dataGridView4.Rows[index].Cells[0].Value.ToString();
                string g = dataGridView4.Rows[index].Cells[5].Value.ToString();

                if (MessageBox.Show(s + "의 아이디를 삭제하시겠습니까", "ID 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (g == "관리자")
                    {
                        MessageBox.Show("관리자는 지울 수 없습니다.");
                    }
                    else
                    {
                        pEOPLEBindingSource.RemoveCurrent();
                        this.pEOPLEBindingSource.EndEdit();
                        this.pEOPLETableAdapter.Update(this.dataSet1.PEOPLE);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button20_Click(object sender, EventArgs e) // 유저관리 : 블랙리스트
        {
            try
            {
                int index = dataGridView4.CurrentCell.RowIndex;
                string s = dataGridView4.Rows[index].Cells[0].Value.ToString();
                string g = dataGridView4.Rows[index].Cells[5].Value.ToString();
                string b = dataGridView4.Rows[index].Cells[6].Value.ToString();

                if (g == "고객")
                {
                    if (b == "10")
                    {
                        if (MessageBox.Show(s + "를 블랙리스트 해제 하시겠습니까?", "블랙리스트 해제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            oracleConnection1.Open();
                            oracleCommand1.CommandText = "UPDATE PEOPLE SET BLACKLIST = '0' WHERE ID = '" + s + "'";
                            oracleCommand1.ExecuteNonQuery();
                            dataGridView4.Rows[index].Cells[6].Value = "0";
                            MessageBox.Show(s + "를 블랙리스트 해제 완료했습니다.");
                            oracleConnection1.Close();
                        }
                    }
                    else if (MessageBox.Show(s + "를 블랙리스트로 변경 하시겠습니까?", "블랙리스트", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        oracleConnection1.Open();
                        oracleCommand1.CommandText = "UPDATE PEOPLE SET BLACKLIST = '10' WHERE ID = '" + s + "'";
                        oracleCommand1.ExecuteNonQuery();
                        dataGridView4.Refresh();
                        dataGridView4.Rows[index].Cells[6].Value = "10";
                        MessageBox.Show(s + "를 블랙리스트로 변경 완료했습니다.");
                        oracleConnection1.Close();
                        pEOPLETableAdapter.Fill(dataSet1.PEOPLE);
                    }
                }
                else
                {
                    MessageBox.Show("고객이 아닙니다.");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)   // 회원가입 : id 변경
        {
            try
            {
                idFlag = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button21_Click(object sender, EventArgs e) // 내정보 : 예약 취소
        {
            try
            {
                int index = dataGridView3.CurrentCell.RowIndex;
                int during = Convert.ToInt32(dataGridView3.Rows[index].Cells[4].Value);
                int cost = Convert.ToInt32(dataGridView3.Rows[index].Cells[3].Value);
                string model = dataGridView3.Rows[index].Cells[0].Value.ToString();
                string reserve_no = dataGridView3.Rows[index].Cells[5].Value.ToString();
                string detail_no = dataGridView3.Rows[index].Cells[9].Value.ToString();

                oracleConnection1.Open();
                int return_cost = cost / 2;

                if (MessageBox.Show(model + "를 예약 취소하시겠습니까? 예약취소비용은 50%입니다. 취소시 반환되는 금액은 " + return_cost + "원 입니다.", "예약 취소", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    vIEWRESERVEBindingSource.RemoveCurrent();
                    this.vIEWRESERVEBindingSource.EndEdit();

                    oracleCommand1.CommandText = "SELECT ID FROM RESERVE WHERE RESERVE_NO = " + reserve_no;
                    string s = Convert.ToString(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "SELECT USED_FEE FROM PEOPLE WHERE ID = '" + s + "'";
                    int cur_fee = Convert.ToInt32(oracleCommand1.ExecuteScalar()) - return_cost;

                    oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = " + cur_fee + " WHERE id = '" + s + "'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE RESERVE SET RESULT = '취소' WHERE RESERVE_NO = " + reserve_no;
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '렌트중' WHERE DETAIL_NO = " + detail_no;
                    oracleCommand1.ExecuteNonQuery();

                    MessageBox.Show("예약 취소 완료!");
                }
                oracleConnection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button22_Click(object sender, EventArgs e) // 렌트 : 반납완료
        {
            try
            {
                int index = dataGridView2.CurrentCell.RowIndex;
                int rent_no = Convert.ToInt32(dataGridView2.Rows[index].Cells["RENT_NO"].Value);
                string vehicle_no = dataGridView2.Rows[index].Cells[7].Value.ToString();
                string detail_no = dataGridView2.Rows[index].Cells[8].Value.ToString();
                string model = dataGridView2.Rows[index].Cells[0].Value.ToString();
                string state = dataGridView2.Rows[index].Cells[2].Value.ToString();
                string cost = dataGridView2.Rows[index].Cells[3].Value.ToString();
                string rent_state = dataGridView2.Rows[index].Cells[9].Value.ToString();
                DateTime rent_end = Convert.ToDateTime(dataGridView2.Rows[index].Cells[5].Value);

                if (comboBox7.Text == "")
                {
                    MessageBox.Show("반납 차량의 상태를 입력해주세요");
                }
                else
                {
                    int pre = 0;
                    int cur = 0;
                    int sub = 0;

                    for (int i = 0; i < 5; ++i)
                    {
                        if (stateArr[i] == state)
                        {
                            pre = i;
                        }
                        if (stateArr[i] == comboBox7.Text)
                        {
                            cur = i;
                        }
                    }

                    sub = pre - cur;

                    oracleConnection1.Open();

                    oracleCommand1.CommandText = "SELECT ORIGINAL_PRICE FROM VEHICLE WHERE VEHICLE_NO = " + vehicle_no;
                    int original_price = Convert.ToInt32(oracleCommand1.ExecuteScalar());
                    int detail_price = original_price - (original_price / 10) * (4 - cur);

                    oracleCommand1.CommandText = "SELECT ID FROM RENT WHERE RENT_NO = " + rent_no;
                    string s = Convert.ToString(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "SELECT DURING FROM RESERVE WHERE DETAIL_NO = " + detail_no;
                    int during = Convert.ToInt32(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "SELECT BLACKLIST FROM PEOPLE WHERE ID = '" + s + "'";
                    string b = Convert.ToString(oracleCommand1.ExecuteScalar());

                    if (MessageBox.Show(s + "님이 " + model + "을 반납하였습니까?", "반납", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (sub > 0)
                        {
                            int sum = Convert.ToInt32(b) + sub;
                            if (sum >= 10)
                            {
                                b = "10";
                            }
                            else
                            {
                                b = sum.ToString();
                            }
                        }

                        oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + detail_price + " WHERE DETAIL_NO = " + detail_no;
                        oracleCommand1.ExecuteNonQuery();

                        oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET STATE = '" + comboBox7.Text + "' WHERE DETAIL_NO = " + detail_no;
                        oracleCommand1.ExecuteReader();

                        oracleCommand1.CommandText = "UPDATE PEOPLE SET BLACKLIST = '" + b + "' WHERE ID = '" + s + "'";
                        oracleCommand1.ExecuteReader();

                        oracleCommand1.CommandText = "UPDATE RENT SET RETURN_M = '" + id + "' WHERE RENT_NO = " + rent_no;
                        oracleCommand1.ExecuteReader();

                        oracleCommand1.CommandText = "UPDATE RENT SET RESULT = '완료' WHERE RENT_NO = " + rent_no;
                        oracleCommand1.ExecuteReader();

                        vIEWRENTBindingSource.RemoveCurrent();
                        this.vIEWRENTBindingSource.EndEdit();


                        if (rent_state == "예약중")
                        {

                            oracleCommand1.CommandText = "SELECT RENT_SEQ.nextval FROM DUAL";
                            int count = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                            string s_date = rent_end.AddDays(1).ToString("yyyy-MM-dd");
                            string e_date = rent_end.AddDays(1 + during).ToString("yyyy-MM-dd");

                            oracleCommand1.CommandText = "SELECT ID FROM RESERVE WHERE DETAIL_NO = " + detail_no + " AND RESULT = '예약중'";
                            string s2 = Convert.ToString(oracleCommand1.ExecuteScalar());

                            oracleCommand1.CommandText = "UPDATE PEOPLE SET RENT_COUNT = RENT_COUNT + 1 WHERE ID = '" + s2 + "'";
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "SELECT PRICE FROM RESERVE WHERE DETAIL_NO = " + detail_no + " AND RESULT = '예약중'";
                            int price = Convert.ToInt32(oracleCommand1.ExecuteScalar());

                            oracleCommand1.CommandText = "INSERT INTO RENT VALUES (" + count + ", '" + s2 + "', " + vehicle_no + ", TO_DATE('" + s_date + "','yyyy-mm-dd'), TO_DATE('" + e_date + "','yyyy-mm-dd'), '" + model + "', " + detail_no + ", '신청중',  " + price + ", ' ', ' ')";    // 추가
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '신청중' WHERE DETAIL_NO = " + detail_no;
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE RESERVE SET RESULT = '완료' WHERE DETAIL_NO = " + detail_no + " AND RESULT = '예약중'";
                            oracleCommand1.ExecuteReader();
                        }
                        else if (rent_state == "렌트중")
                        {
                            oracleCommand1.CommandText = "UPDATE VEHICLE SET CUR_COUNT = CUR_COUNT + 1 WHERE VEHICLE_NO = " + vehicle_no;
                            oracleCommand1.ExecuteNonQuery();

                            oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '대기중' WHERE DETAIL_NO = " + detail_no;
                            oracleCommand1.ExecuteNonQuery();
                        }

                        oracleCommand1.CommandText = "SELECT REVIEW_SEQ.nextval FROM DUAL";
                        int review_no = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                        oracleCommand1.CommandText = "SELECT SYSDATE FROM DUAL";
                        DateTime date = Convert.ToDateTime(oracleCommand1.ExecuteScalar());
                        string return_date = date.ToString("yyyy-MM-dd");

                        oracleCommand1.CommandText = "SELECT V_NO FROM VEHICLE WHERE VEHICLE_NO = " + vehicle_no;
                        int v_no = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                        oracleCommand1.CommandText = "INSERT INTO REVIEW VALUES (" + review_no + ", " + detail_no + ", " + vehicle_no + ", '', TO_DATE('" + return_date + "','yyyy-mm-dd'), " + v_no + ", 0 , '" + model + "', '" + s + "')";    // 추가
                        oracleCommand1.ExecuteNonQuery();

                        int yeaSub = date.Year - rent_end.Year;
                        int monSub = date.Month - rent_end.Month;
                        int daySub = date.Day - rent_end.Day;
                        int passed_day = (yeaSub * 365) + (monSub * 30) + daySub;

                        if (passed_day > 0)
                        {
                            oracleCommand1.CommandText = "SELECT USED_FEE FROM PEOPLE WHERE ID = '" + s + "'";
                            string cur_fee = oracleCommand1.ExecuteScalar().ToString();

                            oracleCommand1.CommandText = "SELECT LATE_FEE FROM PEOPLE WHERE ID = '" + s + "'";
                            string late_fee = oracleCommand1.ExecuteScalar().ToString();

                            string add_fee = (Convert.ToInt32(cost) * passed_day * 2).ToString();
                            if (MessageBox.Show(passed_day.ToString() + "일 연체했습니다. 연체료 " + add_fee + "원을 지불했습니까?.", "연체료", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                cur_fee = (Convert.ToInt32(cur_fee) + Convert.ToInt32(add_fee)).ToString();
                                oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = " + Convert.ToInt32(cur_fee) + " WHERE ID = '" + id + "'";
                                oracleCommand1.ExecuteNonQuery();
                                MessageBox.Show("지불 완료!");
                            }
                            else
                            {
                                cur_fee = (Convert.ToInt32(late_fee) + Convert.ToInt32(add_fee)).ToString();
                                oracleCommand1.CommandText = "UPDATE PEOPLE SET LATE_FEE = " + Convert.ToInt32(cur_fee) + " WHERE ID = '" + id + "'";
                                oracleCommand1.ExecuteNonQuery();
                                MessageBox.Show("연체료가 추가되었습니다.");
                            }
                        }
                        fill();
                        MessageBox.Show("반납 완료!");
                    }
                    oracleConnection1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e) // 차량관리 : 타입 클릭
        {
            try
            {
                int index = dataGridView5.CurrentCell.RowIndex;
                int v_no = Convert.ToInt32(dataGridView5.Rows[index].Cells[1].Value);
                string type = dataGridView5.Rows[index].Cells[0].Value.ToString();

                textBox16.Text = type;
                textBox17.Text = "";
                textBox18.Text = "";
                textBox19.Text = "";

                vEHICLEBindingSource.Filter = "V_NO = " + v_no;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e) // 차량관리 : 모델 클릭
        {
            try
            {
                int index = dataGridView6.CurrentCell.RowIndex;
                int vehicle_no = Convert.ToInt32(dataGridView6.Rows[index].Cells[4].Value.ToString());
                string model = dataGridView6.Rows[index].Cells[0].Value.ToString();
                int capacity = Convert.ToInt32(dataGridView6.Rows[index].Cells[1].Value);
                int cur_count = Convert.ToInt32(dataGridView6.Rows[index].Cells[6].Value);
                int weight = Convert.ToInt32(dataGridView6.Rows[index].Cells[2].Value);

                textBox17.Text = model;
                textBox18.Text = capacity.ToString();
                textBox19.Text = weight.ToString();

                vEHICLEDETAILBindingSource.Filter = "VEHICLE_NO = " + vehicle_no;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button23_Click(object sender, EventArgs e) // 로그인 : 차량관리
        {
            try
            {
                panel_close();
                panel7.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button24_Click(object sender, EventArgs e) // 차량관리 : 추가
        {
            try
            {
                int count;
                int index = dataGridView7.CurrentCell.RowIndex;
                int detail_no = Convert.ToInt32(dataGridView7.Rows[index].Cells[0].Value);
                int vehicle_no = Convert.ToInt32(dataGridView7.Rows[index].Cells[7].Value);
                int v_no = Convert.ToInt32(dataGridView7.Rows[index].Cells[6].Value);

                if (textBox16.Text == "" || textBox17.Text == "" || comboBox5.Text == "" || comboBox10.Text == "")
                {
                    MessageBox.Show("추가할 항목들을 채워주세요.");
                }
                else
                {
                    oracleConnection1.Open();

                    oracleCommand1.CommandText = "SELECT ORIGINAL_PRICE FROM VEHICLE WHERE VEHICLE_NO = " + vehicle_no;
                    int original_price = Convert.ToInt32(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "SELECT VEHICLE_DETAIL_SEQ.nextval FROM DUAL";
                    count = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "INSERT INTO VEHICLE_DETAIL VALUES (" + count + ", " + vehicle_no + ", " + v_no + ", '" + comboBox5.Text + "', " + original_price + ", '매우 좋음', '대기중', '" + comboBox10.Text + "')";
                    oracleCommand1.ExecuteNonQuery();

                    oracleConnection1.Close();

                    fill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                panel_close();
                panel3.Visible = true;

                textBox16.Text = "";
                textBox17.Text = "";
                textBox18.Text = "";
                textBox19.Text = "";
                comboBox5.SelectedIndex = 0;
                comboBox10.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } // 차량관리 : 뒤로가기

        private void button26_Click(object sender, EventArgs e) // 차량관리 : 삭제
        {
            try
            {
                int index = dataGridView7.CurrentCell.RowIndex;
                int vehicle_no = Convert.ToInt32(dataGridView7.Rows[index].Cells[7].Value);
                int detail_no = Convert.ToInt32(dataGridView7.Rows[index].Cells[0].Value);
                string model = textBox17.Text;


                if (MessageBox.Show(model + "를 삭제하시겠습니까", "차량 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    vEHICLEDETAILBindingSource.RemoveCurrent();
                    this.vEHICLEDETAILBindingSource.EndEdit();
                    this.vEHICLE_DETAILTableAdapter.Update(this.dataSet1.VEHICLE_DETAIL);

                    textBox16.Text = "";
                    textBox17.Text = "";
                    textBox18.Text = "";
                    textBox19.Text = "";
                    comboBox5.SelectedIndex = 0;
                    comboBox10.SelectedIndex = 0;

                    fill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button27_Click(object sender, EventArgs e) // 유저관리 : 연체료 납부
        {
            try
            {
                bool flag = false;
                int index = dataGridView4.CurrentCell.RowIndex;
                string id = dataGridView4.Rows[index].Cells[0].Value.ToString();
                string late_fee = dataGridView4.Rows[index].Cells[8].Value.ToString();
                string cost = textBox26.Text;

                oracleConnection1.Open();
                if (cost != "")
                {
                    for (int i = 0; i < cost.Length; ++i)
                    {
                        if (cost[i] < '0' || cost[i] > '9')
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        MessageBox.Show("숫자만 입력해주세요.");
                    }
                    else if (Convert.ToInt32(late_fee) - Convert.ToInt32(cost) < 0)
                    {
                        MessageBox.Show("연체를 한 금액보다 많이 입력하였습니다. 확인 후 다시 입력해주세요.");
                    }
                    else
                    {
                        int sub = Convert.ToInt32(late_fee) - Convert.ToInt32(cost);

                        oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = USED_FEE + " + cost + " WHERE ID = '" + id + "'";
                        oracleCommand1.ExecuteNonQuery();

                        oracleCommand1.CommandText = "UPDATE PEOPLE SET LATE_FEE = " + sub + " WHERE ID = '" + id + "'";
                        oracleCommand1.ExecuteNonQuery();
                        MessageBox.Show(textBox26.Text + "원 납부 완료!");
                    }
                }
                oracleConnection1.Close();

                textBox26.Text = "";

                fill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button28_Click(object sender, EventArgs e) // 유저관리 : 전체
        {
            try
            {
                int index = dataGridView4.CurrentCell.RowIndex;
                string late_fee = dataGridView4.Rows[index].Cells[8].Value.ToString();

                textBox26.Text = late_fee;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e) // 리뷰 : 분류 선택
        {
            try
            {
                int index = category_load(comboBox8.Text);
                string s = textBox28.Text;

                if (index == 0)
                {
                    if (textBox28.Text == "")
                    {
                        rEVIEWBindingSource.Filter = "MARK <> 0";
                    }
                    else
                    {
                        rEVIEWBindingSource.Filter = "MODEL LIKE '%" + s + "%' AND MARK <> 0";
                    }
                }
                else
                {
                    if (textBox28.Text == "")
                    {
                        rEVIEWBindingSource.Filter = "V_NO = " + index + " AND MARK <> 0";
                    }
                    else
                    {
                        rEVIEWBindingSource.Filter = "MODEL LIKE '%" + s + "%' AND V_NO = " + index + " AND MARK <> 0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button29_Click(object sender, EventArgs e) // 리뷰 : 삭제
        {
            try
            {
                int index = dataGridView8.CurrentCell.RowIndex;
                int r_no = Convert.ToInt32(dataGridView8.Rows[index].Cells[4].Value);

                if (MessageBox.Show("선택한 리뷰를 삭제하시겠습니까", "리뷰 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    textBox22.Text = "";

                    rEVIEWBindingSource.RemoveCurrent();
                    this.rEVIEWBindingSource.EndEdit();
                    this.rEVIEWTableAdapter.Update(this.dataSet1.REVIEW);

                    fill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button30_Click(object sender, EventArgs e) // 내정보 : 리뷰 추가
        {
            try
            {
                int index = dataGridView9.CurrentCell.RowIndex;
                int r_no = Convert.ToInt32(dataGridView9.Rows[index].Cells[0].Value);
                string content = textBox27.Text;

                if (comboBox9.Text == "")
                {
                    MessageBox.Show("만족도를 등록해주세요.");
                }
                else
                {
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "UPDATE REVIEW SET CONTENT = '" + content + "' WHERE R_NO = '" + r_no + "'";
                    oracleCommand1.ExecuteNonQuery();
                    oracleCommand1.CommandText = "UPDATE REVIEW SET MARK = '" + comboBox9.Text + "' WHERE R_NO = '" + r_no + "'";
                    oracleCommand1.ExecuteNonQuery();
                    oracleConnection1.Close();

                    textBox27.Text = "";
                    comboBox9.SelectedIndex = 0;
                    MessageBox.Show("수정 완료");
                }

                fill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView9_CellClick(object sender, DataGridViewCellEventArgs e)    // 내 정보 : 리뷰 클릭
        {
            try
            {
                int index = dataGridView9.CurrentCell.RowIndex;
                int r_no = Convert.ToInt32(dataGridView9.Rows[index].Cells[0].Value);
                textBox27.Text = dataGridView9.Rows[index].Cells[4].Value.ToString();
                int comboIndex = Convert.ToInt32(dataGridView9.Rows[index].Cells[7].Value);
                comboBox9.SelectedIndex = comboIndex;

                textBox22.Text = dataGridView8.Rows[0].Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button31_Click(object sender, EventArgs e) // 로그인 : 리뷰 보기
        {
            try
            {
                panel_close();
                panel8.Visible = true;

                rEVIEWBindingSource.Filter = "MARK <> 0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView8_CellClick(object sender, DataGridViewCellEventArgs e)    // 리뷰 보기 : 리뷰 클릭
        {
            try
            {
                int index = dataGridView8.CurrentCell.RowIndex;
                int r_no = Convert.ToInt32(dataGridView8.Rows[index].Cells[4].Value);
                textBox22.Text = dataGridView8.Rows[index].Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button32_Click(object sender, EventArgs e) // 리뷰 보기 : 뒤로가기
        {
            try
            {
                panel_close();
                panel3.Visible = true;

                comboBox8.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)   // 렌트 : 인기순 정렬
        {
            try
            {
                dataGridView1.Sort(DETAIL_NO, ListSortDirection.Ascending);
                if (checkBox3.Checked)
                {
                    dataGridView1.Sort(CUR_COUNT, ListSortDirection.Descending);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)    // 렌트 : 필터 장소
        {
            try
            {
                if (comboBox11.Text == "")
                {
                    filter[7] = "x";
                }
                else
                {
                    filter[7] = comboBox11.Text;
                }
                filter_change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox28_TextChanged(object sender, EventArgs e)  // 리뷰 보기 : 모델 필터
        {
            try
            {
                int index = category_load(comboBox8.Text);
                string s = textBox28.Text;

                if (index == 0)
                {
                    if (textBox28.Text == "")
                    {
                        rEVIEWBindingSource.RemoveFilter();
                    }
                    else
                    {
                        rEVIEWBindingSource.Filter = "MODEL LIKE '%" + s + "%'";
                    }
                }
                else
                {
                    if (textBox28.Text == "")
                    {
                        rEVIEWBindingSource.Filter = "V_NO = " + index;
                    }
                    else
                    {
                        rEVIEWBindingSource.Filter = "MODEL LIKE '%" + s + "%' AND V_NO = " + index;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox29_TextChanged(object sender, EventArgs e)   // 내정보 : ID 입력 (X)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button33_Click(object sender, EventArgs e) // 로그인 : 통계
        {
            try
            {
                panel_close();
                panel9.Visible = true;

                chartWhere = "";
                chartWhere2 = "";

                button34.Text = "원형으로 보기";
                button36.Text = "카테고리 순위 보기";
                chart1.Series["Series1"].Label = "";
                chart1.Series["Series1"].IsValueShownAsLabel = false;
                chart1.Series["Series1"].IsVisibleInLegend = false;
                chart1.Series["Series1"].Points.Clear();
                chart1.ChartAreas["ChartArea1"].AxisY.Title = "매출";

                oracleConnection1.Open();
                oracleCommand1.CommandText = "SELECT VEHICLE_NO, TOTAL FROM (SELECT VEHICLE_NO, SUM(PRICE) TOTAL FROM VIEW_RENT GROUP BY VEHICLE_NO ORDER BY TOTAL DESC) WHERE ROWNUM <= 5";
                OracleDataReader rdr = oracleCommand1.ExecuteReader();
                while (rdr.Read())
                {
                    oracleCommand1.CommandText = "SELECT MODEL FROM VEHICLE WHERE VEHICLE_NO = " + rdr["VEHICLE_NO"];
                    string model = oracleCommand1.ExecuteScalar().ToString();
                    chart1.Series["Series1"].Points.AddXY(model, rdr["TOTAL"]);
                }
                oracleConnection1.Close();

                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;


                chart2.Series["Series1"].Points.Clear();

                oracleConnection1.Open();

                oracleCommand1.CommandText = "SELECT SYSDATE FROM DUAL";
                DateTime nowDate = Convert.ToDateTime(oracleCommand1.ExecuteScalar());

                oracleCommand1.CommandText = "SELECT RENT_START, COUNT(rent_no) COUNT FROM RENT WHERE TO_CHAR(RENT_START, 'MM') = '" + nowDate.Month.ToString() + "' AND TO_CHAR(RENT_START, 'yyyy') = '" + nowDate.Year.ToString() + "' GROUP BY RENT_START ORDER BY RENT_START";
                OracleDataReader rdr2 = oracleCommand1.ExecuteReader();
                while (rdr2.Read())
                {
                    DateTime dt = Convert.ToDateTime(rdr2["RENT_START"]);
                    chart2.Series["Series1"].Points.AddXY(dt.Day.ToString() + "일", rdr2["COUNT"]);
                }
                oracleConnection1.Close();

                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;

                oracleConnection1.Open();

                oracleCommand1.CommandText = "SELECT ID, USE FROM (SELECT ID, SUM(USED_FEE) USE FROM PEOPLE GROUP BY ID ORDER BY USE DESC) WHERE ROWNUM <= 10";

                int i = 1;
                int use;
                string id;

                OracleDataReader rdr3 = oracleCommand1.ExecuteReader();
                while (rdr3.Read())
                {
                    use = Convert.ToInt32(rdr3["USE"]);
                    id = rdr3["ID"].ToString();

                    if (use == 0)
                    {
                        break;
                    }

                    listBox1.Items.Add(i + "등\t" + id + "님        \t" + use + "원 사용!\t");
                    ++i;
                }
                oracleConnection1.Close();

                oracleConnection1.Open();
                oracleCommand1.CommandText = "SELECT SYSDATE FROM DUAL";
                DateTime date = Convert.ToDateTime(oracleCommand1.ExecuteScalar());
                oracleConnection1.Close();

                string date1 = date.ToString("yyyy/MM/dd");

                DateTime date2 = Convert.ToDateTime(date1);

                string date3 = date.ToString("yyyyMMdd");

                vIEWRENTBindingSource3.Filter = "RENT_START = '" + date2 + "' AND RESULT = '완료'";

                oracleConnection1.Open();
                oracleCommand1.CommandText = "SELECT SUM(PRICE) FROM RENT WHERE RENT_START = TO_DATE('" + date3 + "', 'yyyyMMdd') AND RESULT = '완료'";

                string sum = "0";
                sum = oracleCommand1.ExecuteScalar().ToString();
                oracleConnection1.Close();

                label8.Text = date1 + "의 매출액은 " + sum + "원 입니다.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button35_Click(object sender, EventArgs e) // 통계 : 뒤로가기
        {
            try
            {
                panel_close();
                panel3.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button34_Click(object sender, EventArgs e) // 통계 : 1 원형으로 보기
        {
            try
            {
                if (button34.Text == "원형으로 보기")
                {
                    button34.Text = "바 모양으로 보기";
                    chart1.Series["Series1"].Label = "#PERCENT";
                    chart1.Series["Series1"].LegendText = "#VALX";
                    chart1.Series["Series1"].IsValueShownAsLabel = true;
                    chart1.Series["Series1"].IsVisibleInLegend = true;
                    chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "";
                }
                else
                {
                    button34.Text = "원형으로 보기";
                    chart1.Series["Series1"].Label = "";
                    chart1.Series["Series1"].IsValueShownAsLabel = false;
                    chart1.Series["Series1"].IsVisibleInLegend = false;
                    chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
                    chart1.Series["Series1"].LegendText = "";
                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "매출";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button36_Click(object sender, EventArgs e) // 통계 : 1 카테고리 모델 토글버튼
        {
            try
            {
                if (button36.Text == "카테고리 순위 보기")
                {
                    button36.Text = "모델 순위 보기";

                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT TYPE, TOTAL FROM (SELECT v_type.type, SUM(PRICE) TOTAL FROM V_TYPE, VEHICLE, RENT WHERE v_type.v_no = vehicle.v_no AND rent.vehicle_no = vehicle.vehicle_no " + chartWhere + " GROUP BY v_type.type ORDER BY TOTAL DESC) WHERE ROWNUM <= 10";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart1.Series["Series1"].Points.AddXY(rdr["TYPE"], rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
                else
                {
                    button36.Text = "카테고리 순위 보기";

                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT VEHICLE_NO, TOTAL FROM (SELECT VEHICLE_NO, SUM(PRICE) TOTAL FROM VIEW_RENT " + chartWhere2 + " GROUP BY VEHICLE_NO ORDER BY TOTAL DESC) WHERE ROWNUM <= 5";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        oracleCommand1.CommandText = "SELECT MODEL FROM VEHICLE WHERE VEHICLE_NO = " + rdr["VEHICLE_NO"];
                        string model = oracleCommand1.ExecuteScalar().ToString();
                        chart1.Series["Series1"].Points.AddXY(model, rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)   // 통계 : 1 시작 시간
        {
            try
            {
                DateTime dt1 = dateTimePicker1.Value;
                DateTime dt2 = dateTimePicker2.Value;
                string s = dt1.ToString("yyyyMMdd");
                string s2 = dt2.ToString("yyyyMMdd");

                chartWhere = "AND RENT_START >= TO_DATE('" + s + "', 'yyyyMMdd') AND RENT_START <= TO_DATE('" + s2 + "', 'yyyyMMdd')";
                chartWhere2 = "WHERE RENT_START >= TO_DATE('" + s + "', 'yyyyMMdd') AND RENT_START <= TO_DATE('" + s2 + "', 'yyyyMMdd')";

                if (button36.Text == "모델 순위 보기")
                {
                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT TYPE, TOTAL FROM (SELECT v_type.type, SUM(PRICE) TOTAL FROM V_TYPE, VEHICLE, RENT WHERE v_type.v_no = vehicle.v_no AND rent.vehicle_no = vehicle.vehicle_no " + chartWhere + " GROUP BY v_type.type ORDER BY TOTAL DESC) WHERE ROWNUM <= 10";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart1.Series["Series1"].Points.AddXY(rdr["TYPE"], rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
                else
                {
                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT VEHICLE_NO, TOTAL FROM (SELECT VEHICLE_NO, SUM(PRICE) TOTAL FROM VIEW_RENT " + chartWhere2 + " GROUP BY VEHICLE_NO ORDER BY TOTAL DESC) WHERE ROWNUM <= 5";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        oracleCommand1.CommandText = "SELECT MODEL FROM VEHICLE WHERE VEHICLE_NO = " + rdr["VEHICLE_NO"];
                        string model = oracleCommand1.ExecuteScalar().ToString();
                        chart1.Series["Series1"].Points.AddXY(model, rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)   // 통계 : 1 끝 시간
        {
            try
            {
                DateTime dt1 = dateTimePicker1.Value;
                DateTime dt2 = dateTimePicker2.Value;
                string s = dt1.ToString("yyyyMMdd");
                string s2 = dt2.ToString("yyyyMMdd");

                chartWhere = "AND RENT_START >= TO_DATE('" + s + "', 'yyyyMMdd') AND RENT_START <= TO_DATE('" + s2 + "', 'yyyyMMdd')";
                chartWhere2 = "WHERE RENT_START >= TO_DATE('" + s + "', 'yyyyMMdd') AND RENT_START <= TO_DATE('" + s2 + "', 'yyyyMMdd')";

                if (button36.Text == "모델 순위 보기")
                {
                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT TYPE, TOTAL FROM (SELECT v_type.type, SUM(PRICE) TOTAL FROM V_TYPE, VEHICLE, RENT WHERE v_type.v_no = vehicle.v_no AND rent.vehicle_no = vehicle.vehicle_no " + chartWhere + " GROUP BY v_type.type ORDER BY TOTAL DESC) WHERE ROWNUM <= 10";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart1.Series["Series1"].Points.AddXY(rdr["TYPE"], rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
                else
                {
                    chart1.Series["Series1"].Points.Clear();
                    oracleConnection1.Open();
                    oracleCommand1.CommandText = "SELECT VEHICLE_NO, TOTAL FROM (SELECT VEHICLE_NO, SUM(PRICE) TOTAL FROM VIEW_RENT " + chartWhere2 + " GROUP BY VEHICLE_NO ORDER BY TOTAL DESC) WHERE ROWNUM <= 5";
                    OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        oracleCommand1.CommandText = "SELECT MODEL FROM VEHICLE WHERE VEHICLE_NO = " + rdr["VEHICLE_NO"];
                        string model = oracleCommand1.ExecuteScalar().ToString();
                        chart1.Series["Series1"].Points.AddXY(model, rdr["TOTAL"]);
                    }
                    oracleConnection1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button37_Click(object sender, EventArgs e) // 통계 : 2 일간 렌트량
        {
            try
            {
                chart2.Series["Series1"].Points.Clear();
                oracleConnection1.Open();

                DateTime selectedDate = Convert.ToDateTime(dateTimePicker3.Value);

                oracleCommand1.CommandText = "SELECT RENT_START, COUNT(rent_no) COUNT FROM RENT WHERE TO_CHAR(RENT_START, 'MM') = '" + selectedDate.Month.ToString() + "' AND TO_CHAR(RENT_START, 'yyyy') = '" + selectedDate.Year.ToString() + "' GROUP BY RENT_START ORDER BY RENT_START";
                OracleDataReader rdr = oracleCommand1.ExecuteReader();
                while (rdr.Read())
                {
                    DateTime dt = Convert.ToDateTime(rdr["RENT_START"]);
                    chart2.Series["Series1"].Points.AddXY(dt.Day.ToString() + "일", rdr["COUNT"]);
                }
                oracleConnection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button38_Click(object sender, EventArgs e) // 통계 : 2 주간 렌트량
        {
            try
            {
                chart2.Series["Series1"].Points.Clear();
                oracleConnection1.Open();

                DateTime selectedDate = Convert.ToDateTime(dateTimePicker3.Value);
                DateTime temp1;
                DateTime temp2;
                string temp = selectedDate.ToString("yyyyMMdd");

                oracleCommand1.CommandText = "SELECT to_char(to_date('" + temp + "', 'yyyyMMdd'), 'd') from dual";
                string s = oracleCommand1.ExecuteScalar().ToString();

                int sub1 = 1 - Convert.ToInt32(s);
                int sub2 = 7 - Convert.ToInt32(s);
                temp1 = Convert.ToDateTime(selectedDate.AddDays(sub1));
                temp2 = Convert.ToDateTime(selectedDate.AddDays(sub2));

                string start = temp1.ToString("yyyyMMdd");
                string end = temp2.ToString("yyyyMMdd");

                oracleCommand1.CommandText = "SELECT RENT_START, COUNT(rent_no) COUNT FROM RENT WHERE RENT_START >= TO_DATE('" + start + "', 'yyyyMMdd') AND RENT_START <= TO_DATE('" + end + "', 'yyyyMMdd') GROUP BY RENT_START ORDER BY RENT_START";
                OracleDataReader rdr = oracleCommand1.ExecuteReader();
                while (rdr.Read())
                {
                    DateTime dt = Convert.ToDateTime(rdr["RENT_START"]);
                    chart2.Series["Series1"].Points.AddXY(dt.Day.ToString() + "일", rdr["COUNT"]);
                }
                oracleConnection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button39_Click(object sender, EventArgs e) // 통계 : 3 월간 렌트량
        {
            try
            {
                chart2.Series["Series1"].Points.Clear();
                oracleConnection1.Open();

                DateTime selectedDate = Convert.ToDateTime(dateTimePicker3.Value);

                oracleCommand1.CommandText = "SELECT TO_CHAR(RENT_START, 'MM'), COUNT(rent_no) COUNT FROM RENT WHERE TO_CHAR(RENT_START, 'yyyy') = '" + selectedDate.Year.ToString() + "' GROUP BY TO_CHAR(RENT_START, 'MM') ORDER BY TO_CHAR(RENT_START, 'MM')";
                OracleDataReader rdr = oracleCommand1.ExecuteReader();
                while (rdr.Read())
                {
                    string dt = rdr[0].ToString();
                    chart2.Series["Series1"].Points.AddXY(dt + "월", rdr["COUNT"]);
                }
                oracleConnection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)    // 렌트 : 셀클릭 대여기간 수정
        {
            try
            {
                int index = dataGridView1.CurrentCell.RowIndex;
                int v_no = Convert.ToInt32(dataGridView1.Rows[index].Cells["V_NO"].Value);

                if (v_no < 7)
                {
                    comboBox4.Items.Clear();
                    comboBox4.Items.Add("");
                    comboBox4.Items.Add("1일");
                    comboBox4.Items.Add("2일");
                    comboBox4.Items.Add("3일");
                    comboBox4.Items.Add("4일");
                    comboBox4.Items.Add("5일");
                    comboBox4.Items.Add("6일");
                    comboBox4.Items.Add("7일");
                    comboBox4.Items.Add("8일");
                    comboBox4.Items.Add("9일");
                }
                else
                {
                    comboBox4.Items.Clear();
                    comboBox4.Items.Add("");
                    comboBox4.Items.Add("1일");
                    comboBox4.Items.Add("2일");
                    comboBox4.Items.Add("3일");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button40_Click(object sender, EventArgs e) // 유저관리 : 반납 메일 보내기
        {
            try
            {
                int index = dataGridView4.CurrentCell.RowIndex;
                string email = dataGridView4.Rows[index].Cells["EMAIL"].Value.ToString();
                string name = dataGridView4.Rows[index].Cells[2].Value.ToString();
                send_mail(email, name);
                MessageBox.Show("메일 전송 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button41_Click(object sender, EventArgs e) // 유저관리 : 아이디 검색
        {
            try
            {
                if (checkBox4.Checked)
                {
                    if (textBox29.Text == "")
                    {
                        pEOPLEBindingSource.Filter = "LATE_FEE > 0";
                    }
                    else
                    {
                        pEOPLEBindingSource.Filter = "ID LIKE '%" + textBox29.Text + "%' AND LATE_FEE > 0";
                    }
                }
                else
                {
                    if (textBox29.Text == "")
                    {
                        pEOPLEBindingSource.RemoveFilter();
                    }
                    else
                    {
                        pEOPLEBindingSource.Filter = "ID LIKE '%" + textBox29.Text + "%'";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox4_CheckedChanged_1(object sender, EventArgs e) // 유저관리 : 연체료 여부
        {
            try
            {
                if (checkBox4.Checked)
                {
                    if (textBox29.Text == "")
                    {
                        pEOPLEBindingSource.Filter = "LATE_FEE > 0";
                    }
                    else
                    {
                        pEOPLEBindingSource.Filter = "ID LIKE '%" + textBox29.Text + "%' AND LATE_FEE > 0";
                    }
                }
                else
                {
                    if (textBox29.Text == "")
                    {
                        pEOPLEBindingSource.RemoveFilter();
                    }
                    else
                    {
                        pEOPLEBindingSource.Filter = "ID LIKE '%" + textBox29.Text + "%'";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button42_Click(object sender, EventArgs e) // 차량관리 : 수정
        {
            try
            {
                int index2 = dataGridView6.CurrentCell.RowIndex;
                int index = dataGridView7.CurrentCell.RowIndex;
                string vehicle_no = dataGridView6.Rows[index2].Cells[4].Value.ToString();
                string detail_no = dataGridView7.Rows[index].Cells[0].Value.ToString();

                oracleConnection1.Open();
                if (textBox20.Text != "")
                {
                    oracleCommand1.CommandText = "UPDATE VEHICLE SET ORIGINAL_PRICE = " + textBox20.Text + " WHERE VEHICLE_NO = " + vehicle_no;
                    oracleCommand1.ExecuteNonQuery();

                    vEHICLETableAdapter.Fill(dataSet1.VEHICLE);

                    oracleCommand1.CommandText = "SELECT ORIGINAL_PRICE FROM VEHICLE WHERE VEHICLE_NO = " + vehicle_no;
                    int original_price = Convert.ToInt32(oracleCommand1.ExecuteScalar());

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + (original_price - (original_price / 10) * 0) + " WHERE VEHICLE_NO = " + vehicle_no + " AND STATE = '매우 좋음'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + (original_price - (original_price / 10) * 1) + " WHERE VEHICLE_NO = " + vehicle_no + " AND STATE = '좋음'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + (original_price - (original_price / 10) * 2) + " WHERE VEHICLE_NO = " + vehicle_no + " AND STATE = '보통'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + (original_price - (original_price / 10) * 3) + " WHERE VEHICLE_NO = " + vehicle_no + " AND STATE = '나쁨'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET PRICE = " + (original_price - (original_price / 10) * 4) + " WHERE VEHICLE_NO = " + vehicle_no + " AND STATE = '매우 나쁨'";
                    oracleCommand1.ExecuteNonQuery();
                }
                if (comboBox5.Text != "")
                {
                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET COLOR = '" + comboBox5.Text + "' WHERE DETAIL_NO = " + detail_no;
                    oracleCommand1.ExecuteNonQuery();
                }
                if (comboBox10.Text != "")
                {
                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET LOC = '" + comboBox10.Text + "' WHERE DETAIL_NO = " + detail_no;
                    oracleCommand1.ExecuteNonQuery();
                }
                oracleConnection1.Close();

                fill();

                MessageBox.Show("수정 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button43_Click(object sender, EventArgs e) // 렌트 : 차량 보기
        {
            try
            {
                if (button43.Text == "차량 보기")
                {
                    panel10.Visible = true;

                    button43.Text = "그만 보기";

                    int index = dataGridView1.CurrentCell.RowIndex;
                    string vehicle_no = dataGridView1.Rows[index].Cells["VEHICLE_NO"].Value.ToString();

                    string loc = "..\\..\\Pic\\" + vehicle_no + ".PNG";

                    pictureBox1.Load(loc);
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else
                {
                    panel10.Visible = false;

                    button43.Text = "차량 보기";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox21_TextChanged(object sender, EventArgs e)  // 내 정보 : 신청 정보 - 검색
        {
            try
            {
                rENTBindingSource1.Filter = "RESULT = '신청중' AND ID LIKE '%" + textBox21.Text + "%'";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button44_Click(object sender, EventArgs e) // 내 정보 : 신청 정보 - 신청 완료
        {
            try
            {
                int count;
                int index = dataGridView10.CurrentCell.RowIndex;
                string user_id = dataGridView10.Rows[index].Cells[0].Value.ToString();
                string model = dataGridView10.Rows[index].Cells[1].Value.ToString();
                string detail_no = dataGridView10.Rows[index].Cells[6].Value.ToString();
                string used_fee = dataGridView10.Rows[index].Cells[2].Value.ToString();
                string rent_no = dataGridView10.Rows[index].Cells[8].Value.ToString();
                DateTime d1 = Convert.ToDateTime(dataGridView10.Rows[index].Cells[3].Value.ToString());
                DateTime d2 = Convert.ToDateTime(dataGridView10.Rows[index].Cells[4].Value.ToString());

                oracleConnection1.Open();
                oracleCommand1.CommandText = "SELECT USED_FEE FROM PEOPLE WHERE ID = '" + user_id + "'";
                int cur_fee = Convert.ToInt32(oracleCommand1.ExecuteScalar()) - Convert.ToInt32(used_fee);


                if (MessageBox.Show(model + "를 렌트 처리합니다.", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataTable tempTable = dataSet11.Tables["RENT"];
                    DataRow myNewDataRow = tempTable.NewRow();

                    oracleCommand1.CommandText = "SELECT RENT_SEQ.nextval FROM DUAL";
                    count = Convert.ToInt16(oracleCommand1.ExecuteScalar());

                    string date1 = d1.ToString("yyyyMMdd");
                    string date2 = d2.ToString("yyyyMMdd");

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '렌트중' WHERE DETAIL_NO = " + detail_no;
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE RENT SET RESULT = '렌트중' WHERE DETAIL_NO = " + detail_no + " AND RESULT = '신청중'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE RENT SET RENT_M = '" + id + "' WHERE DETAIL_NO = " + detail_no + " AND RESULT = '신청중'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE RENT SET RENT_M = '" + id + "' WHERE RENT_NO = " + rent_no;
                    oracleCommand1.ExecuteReader();

                    MessageBox.Show("렌트 신청 성공");
                }
                else
                {
                    oracleCommand1.CommandText = "UPDATE PEOPLE SET RENT_COUNT = RENT_COUNT - 1 WHERE ID = '" + user_id + "'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE PEOPLE SET USED_FEE = " + cur_fee + " WHERE ID = '" + user_id + "'";
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE VEHICLE_DETAIL SET RENT_STATE = '대기중' WHERE DETAIL_NO = " + detail_no;
                    oracleCommand1.ExecuteNonQuery();

                    oracleCommand1.CommandText = "UPDATE RENT SET RESULT = '취소' WHERE RENT_NO = " + rent_no;
                    oracleCommand1.ExecuteReader();

                    MessageBox.Show("렌트 신청 취소");
                }
                fill();

                oracleConnection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button46_Click(object sender, EventArgs e) // 렌트 기록 : 검색
        {
            try
            {
                if (textBox30.Text == "")
                {
                    if (textBox31.Text == "")
                    {
                        rENTBindingSource2.RemoveFilter();
                    }
                    else
                    {
                        rENTBindingSource2.Filter = "MODEL LIKE '%" + textBox31.Text + "%'";
                    }
                }
                else
                {
                    if (textBox31.Text == "")
                    {
                        rENTBindingSource2.Filter = "ID LIKE '%" + textBox30.Text + "%'";
                    }
                    else
                    {
                        rENTBindingSource2.Filter = "MODEL LIKE '%" + textBox31.Text + "%' AND ID LIKE '%" + textBox30.Text + "%'";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button47_Click(object sender, EventArgs e) // 렌트 기록 : 뒤로 가기
        {
            try
            {
                panel_close();
                panel3.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button48_Click(object sender, EventArgs e) // 로그인 : 렌트 기록
        {
            try
            {
                panel_close();
                panel11.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)   // 통계 : 일일 매출
        {
            try
            {
                DateTime date = dateTimePicker4.Value;

                string date1 = date.ToString("yyyy/MM/dd");

                DateTime date2 = Convert.ToDateTime(date1);

                string date3 = date.ToString("yyyyMMdd");

                vIEWRENTBindingSource3.Filter = "RENT_START = '" + date2 + "' AND RESULT = '완료'";

                oracleConnection1.Open();
                oracleCommand1.CommandText = "SELECT SUM(PRICE) FROM RENT WHERE RENT_START = TO_DATE('" + date3 + "', 'yyyyMMdd') AND RESULT = '완료'";

                string sum = "0";
                sum = oracleCommand1.ExecuteScalar().ToString();
                oracleConnection1.Close();

                label8.Text = date1 + "의 매출액은 " + sum + "원 입니다.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}