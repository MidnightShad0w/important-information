import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import {CSSTransition} from 'react-transition-group';
import SideBar from '../SideBar/SideBar';
import './Main.modules.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import ReactDatePicker from '../DatePicker/DatePicker'




function Main(){
    const [showSB, setShowSB] = useState(false);
    const navigate = useNavigate();

    const UserLogout = () => {
        Cookies.remove('token');
        console.log('After logout token is => ' + Cookies.get('token'))
        navigate('/')
    }

    return (
        <div className="wrapper">
            <div className="shadow-box">
                <header className="header d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom">
                    <h1 className="d-flex align-items-center col-md-3 mb-2 mb-md-0 text-dark text-decoration-none">LOGO</h1>
                    <div className="col-md-3 group-btn">
                        <button type="button" className="btn btn-outline-primary me-2 news-btn" onClick={()=>setShowSB(!showSB)}>Новости</button>
                        <button onClick={UserLogout} className="logout-btn btn btn-primary">Logout</button>
                    </div>
                </header>
            </div>
            <div className="content-wrapper">
                <div className="container">

                    {/* date */}
                    <div className="parent-datepicker-wrapper">
                    <ReactDatePicker />
                    </div>
                    {/* date */}
                    {/* cards */}
                    <div class="row row-cols-1 row-cols-md-3 g-4 cards-wrapper">
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту. Этот контент немного длиннее.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту. Этот контент немного длиннее.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту. Этот контент немного длиннее.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту. Этот контент немного длиннее.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col ">
                            <div class="card text-white bg-secondary mb-3 shadow-outline">
                                <div class="card-body">
                                    <h5 class="card-title">Заголовок карточки</h5>
                                    <p class="card-text">Это более длинная карта С вспомогательным текстом ниже в качестве естественного перехода к дополнительному контенту. Этот контент немного длиннее.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    {/* cards */}
                </div>
                <CSSTransition in={showSB} timeout={300} classNames='fade' unmountOnExit>
                    <div className="SideBarWrapper">
                        <SideBar/>
                    </div>
                </CSSTransition>
            </div>
        </div>
    )
}
export default Main