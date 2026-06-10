import {BrowserRouter, Route, Routes} from 'react-router';
import SignInPage from './pages/SignInPage';
import SignUpPage from './pages/SignUpPage';
import ChatAppPage from './pages/ChatAppPage';
import {Toaster} from "sonner";
function App() {
  

  return (
      <>
      <Toaster/>
        <BrowserRouter>
        <Routes>
        /*Public route */
        <Route
        path='/signin'
        element={<SignInPage/>}
        />

        <Route
        path='/signup'
        element={<SignUpPage/>}
        />

        /*Protected route */

        <Route 
        path='/'
        element={<ChatAppPage/>}
        />
        </Routes>
        </BrowserRouter>
      </>
  )
}

export default App
